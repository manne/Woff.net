using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;

using Mono;
using WoffDotNet.Exceptions;
using WoffDotNet.Readers;
using WoffDotNet.Types;
using WoffDotNet.Validators;

namespace WoffDotNet
{
    public class WoffReader
    {
        private readonly BinaryReader _binaryReader;
        private Queue<WoffTableDirectory> tableDirectories;

        private WoffHeader _header;

        public WoffReader(BinaryReader binaryReader)
        {
            Contract.Requires(binaryReader != null);

            _binaryReader = binaryReader;
        }


        public void Process()
        {
            Contract.Ensures(HeaderState != null);

            ProcessHeader();
            ProcessTableDirectories();
        }

        private void ProcessTableDirectories()
        {
            var tableDirectoriesBlockSize = Header.NumTables * WoffTableDirectory.Size;
            var bytes = new byte[tableDirectoriesBlockSize];
            if (_binaryReader.Read(bytes, (int)WoffHeader.Size, (int)tableDirectoriesBlockSize) != tableDirectoriesBlockSize)
            {
                throw new EndOfStreamException("The stream does not have the table directories");
            }

            tableDirectories = new Queue<WoffTableDirectory>(Header.NumTables);

            int offset = 0;
            for (int i = 0; i < Header.NumTables; i++)
            {
                var directoryBytes = new byte[WoffTableDirectory.Size];
                Array.Copy(bytes, offset, directoryBytes, 0, (int)WoffTableDirectory.Size);
                var directoryReader = new WoffTableDirectoryReader(directoryBytes);
                var woffTableDirectory = directoryReader.Process();
                tableDirectories.Enqueue(woffTableDirectory);
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
    }
}
