using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

using Blocker;

using Ionic.Zlib;

using WoffDotNet.Exceptions;
using WoffDotNet.Readers;
using WoffDotNet.Types;
using WoffDotNet.Validators;

namespace WoffDotNet
{
    /// <summary>
    /// The WOFF reader.
    /// </summary>
    public class WoffReader
    {
        private readonly BinaryReader _binaryReader;

        /// <summary>
        /// The byte boundary.
        /// </summary>
        public const uint ByteBoundary = 4;

        /// <summary>
        /// The maximal padding between blocks.
        /// </summary>
        public const uint MaxPadding = ByteBoundary - 1;

        private List<WoffTableDirectory> _tableDirectories;

        private Dictionary<WoffTableDirectory, Tuple<byte[], byte[]>> _fontTableDictionary;

        private WoffHeader _header;

        private NestedBlock _protocolBlock;

        /// <summary>
        /// Initializes a new instance of the <see cref="WoffReader"/> class.
        /// </summary>
        /// <param name="binaryReader">
        /// The binary reader.
        /// </param>
        public WoffReader(BinaryReader binaryReader)
        {
            Contract.Requires(binaryReader != null);
            Contract.Requires(binaryReader.BaseStream != null);
            Contract.Requires(binaryReader.BaseStream.CanRead);
            Contract.Requires(binaryReader.BaseStream.CanSeek);

            _binaryReader = binaryReader;
        }

        /// <summary>
        /// Process the reading of the WOFF file
        /// </summary>
        /// <exception cref="AggregateException">
        /// Thrown if there was a severe error.
        /// </exception>
        public void Process()
        {
            Contract.Ensures(HeaderState != null);

            ProcessHeader();
            if (!_protocolBlock.Validate())
            {
                throw new AggregateException("Following exceptions occured", _protocolBlock.Exceptions);
            }

            ProcessTableDirectories();
            if (!_protocolBlock.Validate(true))
            {
                throw new AggregateException("Following exceptions occured", _protocolBlock.Exceptions);
            }

            var minorExceptions = new List<Exception>(10);

            var exceptionsFromProcessingFontTables = ProcessFontTables();
            minorExceptions.AddRange(exceptionsFromProcessingFontTables.InnerExceptions);

            ProcessMetadata();

            CheckPossiblePaddingBetweenMetadataAndPrivateData();

            ProcessPrivateData();

            if (minorExceptions.Any())
            {
                throw new AggregateException(minorExceptions);
            }
        }

        private void CheckPossiblePaddingBetweenMetadataAndPrivateData()
        {
            if (_header.MetaOffset > 0 && _header.PrivOffset > 0)
            {
                var startOfPadding = _header.MetaOffset + _header.MetaLength;
                _binaryReader.BaseStream.Position = startOfPadding;
                var paddingLength = _header.PrivOffset - startOfPadding;
                var bytes = new byte[paddingLength];
                if (_binaryReader.Read(bytes, 0, bytes.Length) != bytes.Length)
                {
                    throw new EndOfStreamException("Could not padding between metadata and private data");
                }

                if (!WoffHelpers.IsNullByteArray(bytes))
                {
                    throw new InvalidNullPaddingException("The bytes between metadata and private data are not padded with zero bytes");
                }
            }
        }

        private void ProcessPrivateData()
        {
            if (!Header.HasPrivateDate())
            {
                return;
            }

            _binaryReader.BaseStream.Position = Header.PrivOffset;
            var bytes = new byte[Header.PrivLength];
            if (_binaryReader.Read(bytes, 0, bytes.Length) != bytes.Length)
            {
                throw new EndOfStreamException("Could not read private data");
            }

            PrivateData = bytes;
        }

        private void ProcessMetadata()
        {
            if (!Header.HasMetadata())
            {
                return;
            }

            _binaryReader.BaseStream.Position = Header.MetaOffset;
            var bytes = new byte[Header.MetaLength];
            if (_binaryReader.Read(bytes, 0, bytes.Length) != bytes.Length)
            {
                throw new EndOfStreamException("Could not read metadata");
            }

            var reader = new WoffMetadataReader(bytes, Header);
            reader.Process();
            Metadata = reader.Document;
            MetadataExceptions = reader.Exceptions;
        }

        private AggregateException ProcessFontTables()
        {
            _fontTableDictionary = new Dictionary<WoffTableDirectory, Tuple<byte[], byte[]>>(_tableDirectories.Count);
            var result = new List<Exception>();

            for (int i = 0; i < _tableDirectories.Count; i++)
            {
                var tableDirectory = _tableDirectories[i];

                _binaryReader.BaseStream.Position = tableDirectory.Offset;

                var bytes = new byte[tableDirectory.CompLength];
                if (_binaryReader.Read(bytes, 0, bytes.Length) != bytes.Length)
                {
                    throw new EndOfStreamException();
                }

                var uncompressedFontTable = bytes;
                if (tableDirectory.CompLength != tableDirectory.OrigLength)
                {
                    try
                    {
                        uncompressedFontTable = ZlibStream.UncompressBuffer(bytes);
                    }
                    catch (ZlibException e)
                    {
                        throw new WoffUncompressException("Could not decompress font table", e);
                    }
                }

                if (uncompressedFontTable.Length != tableDirectory.OrigLength)
                {
                    throw new InvalidRangeException(string.Format(CultureInfo.InvariantCulture, "The stated font table length {0} is not equal to the actual value {1}", tableDirectory.OrigLength, uncompressedFontTable.Length));
                }

                if (tableDirectory.CompLength > tableDirectory.OrigLength)
                {
                    result.Add(new InvalidRangeException(string.Format(CultureInfo.InvariantCulture, "The \"{0}\" table directory entry has a compressed length ({1}) larger than the original length ({2}).", tableDirectory.TagAsString(), tableDirectory.CompLength, tableDirectory.OrigLength)));
                }

                if (tableDirectory.Padding > 0)
                {
                    var paddingBytes = new byte[tableDirectory.Padding];
                    if (_binaryReader.Read(paddingBytes, 0, paddingBytes.Length) != paddingBytes.Length)
                    {
                        throw new EndOfStreamException();
                    }

                    if (!WoffHelpers.IsNullByteArray(paddingBytes))
                    {
                        result.Add(new InvalidNullPaddingException("A table is not padded with null bytes"));
                    }
                }

                _fontTableDictionary.Add(tableDirectory, new Tuple<byte[], byte[]>(bytes, uncompressedFontTable));
            }

            return new AggregateException(result);
        }

        private void ProcessTableDirectories()
        {
            Contract.Ensures(_tableDirectories != null);
            Contract.Ensures(_tableDirectories.Count == Header.NumTables);

            var result = new List<Exception>(5);
            var tableDirectoriesBlockSize = Header.NumTables * WoffTableDirectory.Size;
            var bytes = new byte[tableDirectoriesBlockSize];
            if (_binaryReader.Read(bytes, 0, (int)tableDirectoriesBlockSize) != tableDirectoriesBlockSize)
            {
                throw new EndOfStreamException("The stream does not have the table directories");
            }

            _tableDirectories = new List<WoffTableDirectory>(Header.NumTables);
            var tableDirectoriesBlocks = new List<Block>(Header.NumTables);

            int offset = 0;
            for (var i = 0; i < Header.NumTables; i++)
            {
                var directoryBytes = new byte[WoffTableDirectory.Size];
                Array.Copy(bytes, offset, directoryBytes, 0, (int)WoffTableDirectory.Size);
                var directoryReader = new WoffTableDirectoryReader(directoryBytes);
                var woffTableDirectory = directoryReader.Process();
                _tableDirectories.Add(woffTableDirectory);

                tableDirectoriesBlocks.Add(Block.CreateFromStartAndDistance(woffTableDirectory.Offset, woffTableDirectory.CompLength));
                offset += (int)WoffTableDirectory.Size;
            }

            CheckCorrectOrderOfTableDirectories();

            _tableDirectories.Sort((directory, tableDirectory) => directory.Offset.CompareTo(tableDirectory.Offset));
            tableDirectoriesBlocks.Sort((block, block1) => block.Start.CompareTo(block1.Start));

            WoffTableDirectory lastBlock = _tableDirectories[0];
            for (var i = 0; i < _tableDirectories.Count; i++)
            {
                lastBlock = _tableDirectories[i];

                if (i < _tableDirectories.Count - 1)
                {
                    lastBlock.Padding = BlockExtensions.CalculateNextBytePadding(lastBlock.Offset + lastBlock.CompLength, ByteBoundary);
                }
            }

            var start = WoffHeader.Size + tableDirectoriesBlockSize;
            var end = lastBlock.Offset + lastBlock.CompLength;
            if (start != tableDirectoriesBlocks[0].Start)
            {
                throw new InvalidDataException(string.Format("The table data does not start ({0}) at the required position ({1})", tableDirectoriesBlocks[0].Start, start));
            }

            if (Header.MetaOffset > 0 || Header.PrivOffset > 0)
            {
                end += BlockExtensions.CalculateNextBytePadding(end, ByteBoundary);
            }

            var nestedBlock = new NestedBlock(start, end, new NestedBlockOptions(MaxPadding, ByteBoundary));
            nestedBlock.AddRange(tableDirectoriesBlocks.ToArray());
            _protocolBlock.AddChild(nestedBlock);
        }

        private void CheckCorrectOrderOfTableDirectories()
        {
            for (int i = 0; i < _tableDirectories.Count; i++)
            {
                if (i > 0 && i < _tableDirectories.Count)
                {
                    var diff = _tableDirectories[i - 1].Tag.CompareTo(_tableDirectories[i].Tag);
                    if (diff > 0)
                    {
                        throw new InvalidRangeException(string.Format(CultureInfo.InvariantCulture, "The table directory entries are not stored in alphabetical order. {0} {1}", _tableDirectories[i - 1].TagAsString(), _tableDirectories[i].TagAsString()));
                    }
                }
            }
        }

        private void ProcessHeader()
        {
            Contract.Ensures(Header.NumTables > 0);

            var bytes = new byte[WoffHeader.Size];
            if (_binaryReader.Read(bytes, 0, bytes.Length) != WoffHeader.Size)
            {
                throw new EndOfStreamException("The stream does not have a header");
            }

            var headerReader = new WoffHeaderReader(bytes);
            headerReader.Process();
            _header = headerReader.Header;

            if (_header.NumTables == 0)
            {
                throw new InvalidDataException("The header must have at least one font table");
            }

            _protocolBlock = new NestedBlock(0, _header.Length, new NestedBlockOptions(MaxPadding, ByteBoundary));
            _protocolBlock.AddChild(Block.CreateFromStartAndDistance(0, WoffHeader.Size));

            var hasIllegalMetadata = WoffHeaderValidator.HasIllegalMetadata(_header);
            var hasIllegalPrivateData = WoffHeaderValidator.HasIllegalPrivateData(_header);
            HeaderState = new HeaderState(!hasIllegalMetadata, !hasIllegalPrivateData);

            if (Header.MetaOffset == 0 && Header.MetaLength > 0)
            {
                throw new InvalidDataException("The metadata offset is set to zero, but the length is not set to zero");
            }

            if (Header.MetaOffset == 0 && Header.MetaOrigLength > 0)
            {
                throw new InvalidDataException("The metadata offset is set to zero, but the original length is not set to zero");
            }

            if (Header.MetaOffset > 0 && Header.MetaLength == 0)
            {
                throw new InvalidDataException("The metadata length is set to zero, but the meta offset is not set to zero");
            }

            if (Header.MetaOffset > 0 && Header.MetaOrigLength == 0)
            {
                throw new InvalidDataException("The metadata orig length is set to zero, but the meta offset is not set to zero");
            }

            if (Header.PrivOffset == 0 && Header.PrivLength > 0)
            {
                throw new InvalidDataException("The privatedata offset is set to zero, but the privatedata length is not set to zero");
            }

            if (Header.PrivOffset > 0 && Header.PrivLength == 0)
            {
                throw new InvalidDataException("The privatedata length is set to zero, but the privatedata offset is not set to zero");
            }

            if (Header.MetaOffset > 0)
            {
                var metadataBlock = Block.CreateFromStartAndDistance(Header.MetaOffset, Header.MetaLength);
                _protocolBlock.AddChild(metadataBlock);
            }

            if (Header.PrivOffset > 0)
            {
                var privateDataBlock = Block.CreateFromStartAndDistance(Header.PrivOffset, Header.PrivLength);
                _protocolBlock.AddChild(privateDataBlock);
            }

            var tableDirectoryBlock = Block.CreateFromStartAndDistance(WoffHeader.Size, _header.NumTables * WoffTableDirectory.Size);
            _protocolBlock.AddChild(tableDirectoryBlock);
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        public WoffHeader Header
        {
            get
            {
                return _header;
            }
        }

        /// <summary>
        /// Gets the header state.
        /// </summary>
        public HeaderState HeaderState { get; private set; }

        /// <summary>
        /// Gets the metadata.
        /// </summary>
        public XmlDocument Metadata { get; private set; }

        /// <summary>
        /// Gets the exceptions associated with parsing the metadata
        /// </summary>
        public IEnumerable<Exception> MetadataExceptions { get; private set; }

        /// <summary>
        /// Gets the private data
        /// </summary>
        public byte[] PrivateData { get; private set; }
    }
}
