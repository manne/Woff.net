using System;
using System.IO;

using FluentAssertions;

using WoffDotNet.Exceptions;
using WoffDotNet.Tests.Properties;

using Xunit;

namespace WoffDotNet.Tests
{
    public class OfficialW3CTests
    {
        private static WoffReader GetReader(byte[] bytes)
        {
            return new WoffReader(new BinaryReader(new MemoryStream(bytes)));
        }

        [Fact]
        public void Valid_001_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.valid_001);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Valid_001_NoPrivateData()
        {
            // arrange
            var cut = GetReader(Resources.valid_001);

            // act
            cut.Process();

            // assert
            cut.PrivateData.Should().BeNull();
        }

        [Fact]
        public void Valid_002()
        {
            // arrange
            var cut = GetReader(Resources.valid_002);

            // act
            cut.Process();

            // assert
            cut.ShouldHaveCorrectlyAssignedMetadata();
        }

        [Fact]
        public void Valid_003()
        {
            // arrange
            var cut = GetReader(Resources.valid_003);

            // act
            cut.Process();

            // assert
            cut.PrivateData.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Valid_004_WithMetadata()
        {
            // arrange
            var cut = GetReader(Resources.valid_004);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Valid_004_WithPrivateData()
        {
            // arrange
            var cut = GetReader(Resources.valid_004);

            // act
            cut.Process();

            // assert
            cut.PrivateData.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Invalid_Header_Signature_001()
        {
            // arrange
            var cut = GetReader(Resources.header_signature_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidWoffMagicNumberException>();
        }

        [Fact]
        public void Invalid_Header_Reserved_001()
        {
            // arrange
            var cut = GetReader(Resources.header_reserved_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidWoffReservedValueException>();
        }

        [Fact]
        public void Invalid_Header_NumTables_001()
        {
            // arrange
            var cut = GetReader(Resources.header_numTables_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Metadata_MetaOrigLength_001_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_metaOrigLength_001);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_MetaOrigLength_001_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_metaOrigLength_001);

            // act
            cut.Process();

            // assert
           cut.MetadataExceptions.Should().ContainItemsAssignableTo<InvalidRangeException>();
        }

        [Fact]
        public void Invalid_Metadata_MetaOrigLength_002_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_metaOrigLength_002);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_MetaOrigLength_002_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_metaOrigLength_002);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().ContainItemsAssignableTo<InvalidRangeException>();
        }

        [Fact]
        public void Invalid_Metadata_Compression_001()
        {
            // arrange
            var cut = GetReader(Resources.metadata_compression_001);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Valid_Metadata_Encoding_001()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_001);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().NotBeNull();
        }

        [Fact]
        public void Invalid_Metadata_Encoding_002_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_002);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Encoding_002_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_002);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().ContainItemsAssignableTo<EncodingNotSupportedException>();
        }

        [Fact]
        public void Invalid_Metadata_Encoding_003_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_003);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Encoding_003_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_003);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().ContainItemsAssignableTo<EncodingNotSupportedException>();
        }

        [Fact]
        public void Valid_Metadata_Encoding_004()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_004);

            // act
            cut.Process();

            // assert
            cut.ShouldHaveCorrectlyAssignedMetadata();
        }

        [Fact]
        public void Valid_Metadata_Encoding_005()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_005);

            // act
            cut.Process();

            // assert
            cut.ShouldHaveCorrectlyAssignedMetadata();
        }

        [Fact]
        public void Invalid_Metadata_Encoding_006_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_006);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Encoding_006_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_encoding_006);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().ContainItemsAssignableTo<EncodingNotSupportedException>();
        }

        [Fact]
        public void Valid_Metadata_Schema_Metadata_001()
        {
            // arrange
            var cut = GetReader(Resources.metadata_schema_metadata_001);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().NotBeNull();
        }

        [Fact]
        public void Invalid_Metadata_Schema_Metadata_002_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_schema_metadata_002);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Schema_Metadata_002_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_schema_metadata_002);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Invalid_Metadata_Schema_Metadata_003_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_schema_metadata_003);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Schema_Metadata_003_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_schema_metadata_003);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Invalid_Metadata_Schema_Metadata_004_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_schema_metadata_004);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Schema_Metadata_004_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_schema_metadata_004);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Invalid_Directory_OrigLength_001()
        {
            // arrange
            var cut = GetReader(Resources.directory_origLength_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidRangeException>();
        }

        [Fact]
        public void Invalid_Directory_OrigLength_002()
        {
            // arrange
            var cut = GetReader(Resources.directory_origLength_002);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidRangeException>();
        }

        [Fact]
        public void Invalid_TableData_Zlib_001()
        {
            // arrange
            var cut = GetReader(Resources.tabledata_zlib_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<WoffUncompressException>();
        }

        [Fact]
        public void Invalid_Blocks_Metadata_Absent_001()
        {
            // arrange
            var cut = GetReader(Resources.blocks_metadata_absent_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Blocks_Metadata_Absent_002()
        {
            // arrange
            var cut = GetReader(Resources.blocks_metadata_absent_002);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Directory_4_Byte_001()
        {
            // arrange
            var cut = GetReader(Resources.directory_4_byte_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<Exception>();
        }
    }
}
