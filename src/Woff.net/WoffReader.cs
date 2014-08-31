using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Net.Security;
using System.Xml;

using Blocker;

using Ionic.Zlib;

using WoffDotNet.Exceptions;
using WoffDotNet.Readers;
using WoffDotNet.Types;
using WoffDotNet.Validators;

namespace WoffDotNet
{
    public class WoffReader
    {
        private readonly BinaryReader _binaryReader;

        private List<WoffTableDirectory> _tableDirectories;

        private Dictionary<WoffTableDirectory, Tuple<byte[], byte[]>> _fontTableDictionary;

        private WoffHeader _header;

        private NestedBlock _protocolBlock;

        public WoffReader(BinaryReader binaryReader)
        {
            Contract.Requires(binaryReader != null);
            Contract.Requires(binaryReader.BaseStream != null);
            Contract.Requires(binaryReader.BaseStream.CanRead);
            Contract.Requires(binaryReader.BaseStream.CanSeek);

            _binaryReader = binaryReader;
        }


        public void Process()
        {
            Contract.Ensures(HeaderState != null);

            ProcessHeader();
            if (!_protocolBlock.Validate())
            {
                throw new Exception();
            }

            ProcessTableDirectories();
            if (!_protocolBlock.Validate())
            {
                throw new Exception();
            }

            ProcessFontTables();
            ProcessMetadata();
            ProcessPrivateData();
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

        private void ProcessFontTables()
        {
            _fontTableDictionary = new Dictionary<WoffTableDirectory, Tuple<byte[], byte[]>>(_tableDirectories.Count);

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

                _fontTableDictionary.Add(tableDirectory, new Tuple<byte[], byte[]>(bytes, uncompressedFontTable));
            }
        }

        private void ProcessTableDirectories()
        {
            Contract.Ensures(_tableDirectories != null);
            Contract.Ensures(_tableDirectories.Count == Header.NumTables);

            var tableDirectoriesBlockSize = Header.NumTables * WoffTableDirectory.Size;
            var bytes = new byte[tableDirectoriesBlockSize];
            if (_binaryReader.Read(bytes, 0, (int)tableDirectoriesBlockSize) != tableDirectoriesBlockSize)
            {
                throw new EndOfStreamException("The stream does not have the table directories");
            }

            _tableDirectories = new List<WoffTableDirectory>(Header.NumTables);

            int offset = 0;
            for (var i = 0; i < Header.NumTables; i++)
            {
                var directoryBytes = new byte[WoffTableDirectory.Size];
                Array.Copy(bytes, offset, directoryBytes, 0, (int)WoffTableDirectory.Size);
                var directoryReader = new WoffTableDirectoryReader(directoryBytes);
                var woffTableDirectory = directoryReader.Process();
                _tableDirectories.Add(woffTableDirectory);

                _protocolBlock.AddChild(Block.CreateFromStartAndDistance(woffTableDirectory.Offset, woffTableDirectory.CompLength));

                offset += (int)WoffTableDirectory.Size;
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

            _protocolBlock = new NestedBlock(0, _header.Length, new NestedBlockOptions(3, 4));

            var hasIllegalMetadata = WoffHeaderValidator.HasIllegalMetadata(_header);
            var hasIllegalPrivateData = WoffHeaderValidator.HasIllegalPrivateData(_header);
            HeaderState = new HeaderState(!hasIllegalMetadata, !hasIllegalPrivateData);

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

        public WoffHeader Header
        {
            get
            {
                return _header;
            }
        }

        public HeaderState HeaderState { get; private set; }

        public XmlDocument Metadata { get; private set; }

        public IEnumerable<Exception> MetadataExceptions { get; private set; }

        public byte[] PrivateData { get; private set; }
    }
}
