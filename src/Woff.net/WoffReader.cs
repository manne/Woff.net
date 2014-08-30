using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using System.IO;
using System.Linq;
using System.Xml;

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

        private Dictionary<WoffTableDirectory, byte[]> _fontTableDictionary;

        private WoffHeader _header;

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
            ProcessTableDirectories();
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
            var hasInvalidLengths = false;
            if (_binaryReader.Read(bytes, 0, bytes.Length) != bytes.Length)
            {
                throw new EndOfStreamException("Could not read metadata");
            }

            var exceptions = new List<Exception>();
            var metaBytes = bytes;
            if (Header.MetaLength <= Header.MetaOrigLength)
            {
                try
                {
                    metaBytes = ZlibStream.UncompressBuffer(bytes);
                    hasInvalidLengths = !WoffMetadataValidator.ValidateLengths(Header.MetaOrigLength, (uint)metaBytes.Length);
                }
                catch (ZlibException e)
                {
                    exceptions.Add(new WoffUncompressException("Cannot uncompress metadata", e));
                }
            }

            var xmlDocument = new XmlDocument();
            var memoryStream = new MemoryStream(metaBytes);
            using (var reader = XmlReader.Create(memoryStream))
            {
                xmlDocument.Load(reader);
            }

            var aggregateException = xmlDocument.ValidateWoffMetadata();
            if (aggregateException != null)
            {
                exceptions.AddRange(aggregateException.InnerExceptions);
            }

            if (hasInvalidLengths)
            {
                exceptions.Add(new InvalidRangeException("The stated metadata length is not equal to the actual value"));
            }

            if (exceptions.Any())
            {
                MetadataExceptions = exceptions;
            }
            else
            {
                Metadata = xmlDocument;
            }
        }

        private void ProcessFontTables()
        {
            _fontTableDictionary = new Dictionary<WoffTableDirectory, byte[]>(_tableDirectories.Count);

            for (int i = 0; i < _tableDirectories.Count; i++)
            {
                var tableDirectory = _tableDirectories[i];
                var bytes = new byte[tableDirectory.CompLength];
                if (_binaryReader.Read(bytes, 0, bytes.Length) != bytes.Length)
                {
                    throw new EndOfStreamException();
                }

                _fontTableDictionary.Add(tableDirectory, bytes);
                var position = (uint)_binaryReader.BaseStream.Position;
                var padding = WoffHelpers.Calculate4BytePadding(position);
                _binaryReader.BaseStream.Position = position + padding;
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

            var hasIllegalMetadata = WoffHeaderValidator.HasIllegalMetadata(_header);
            var hasIllegalPrivateData = WoffHeaderValidator.HasIllegalPrivateData(_header);
            HeaderState = new HeaderState(!hasIllegalMetadata, !hasIllegalPrivateData);
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
