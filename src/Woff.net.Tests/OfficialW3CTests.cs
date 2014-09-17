using System;
using System.IO;
using System.Xml;

using Blocker.Exceptions;

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
        public void Invalid_Metadata_Well_Formed_001_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_well_formed_001);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().ContainItemsAssignableTo<XmlException>();
        }

        [Fact]
        public void Invalid_Metadata_Well_Formed_001_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_well_formed_001);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Well_Formed_002_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_well_formed_002);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().ContainItemsAssignableTo<XmlException>();
        }

        [Fact]
        public void Invalid_Metadata_Well_Formed_002_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_well_formed_002);

            // act
            cut.Process();

            // assert
            cut.Metadata.Should().BeNull();
        }

        [Fact]
        public void Invalid_Metadata_Well_Formed_003_CorrectException()
        {
            // arrange
            var cut = GetReader(Resources.metadata_well_formed_003);

            // act
            cut.Process();

            // assert
            cut.MetadataExceptions.Should().ContainItemsAssignableTo<XmlException>();
        }

        [Fact]
        public void Invalid_Metadata_Well_Formed_003_NoMetadata()
        {
            // arrange
            var cut = GetReader(Resources.metadata_well_formed_003);

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
        public void Invalid_Metadata_Padding_001()
        {
            // arrange
            var cut = GetReader(Resources.metadata_padding_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidNullPaddingException>();
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
        public void Invalid_Directory_Ascending_001()
        {
            // arrange
            var cut = GetReader(Resources.directory_ascending_001);

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
        public void Valid_TableData_Compression_001()
        {
            // arrange
            var cut = GetReader(Resources.tabledata_compression_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldNotThrow(because: "the woff is valid");
        }

        [Fact]
        public void Valid_TableData_Compression_002()
        {
            // arrange
            var cut = GetReader(Resources.tabledata_compression_002);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldNotThrow(because: "the woff is valid");
        }

        [Fact]
        public void Valid_TableData_Compression_003()
        {
            // arrange
            var cut = GetReader(Resources.tabledata_compression_003);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldNotThrow(because: "the woff is valid");
        }

        [Fact]
        public void Valid_TableData_Compression_004()
        {
            // arrange
            var cut = GetReader(Resources.tabledata_compression_004);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldNotThrow(because: "the woff is valid");
        }

        [Fact]
        public void Invalid_Blocks_Extraneous_Data_001()
        {
            // arrange
            var cut = GetReader(Resources.blocks_extraneous_data_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Blocks_Extraneous_Data_002()
        {
            // arrange
            var cut = GetReader(Resources.blocks_extraneous_data_002);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Blocks_Extraneous_Data_003()
        {
            // arrange
            var cut = GetReader(Resources.blocks_extraneous_data_003);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Blocks_Extraneous_Data_004()
        {
            // arrange
            var cut = GetReader(Resources.blocks_extraneous_data_004);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Blocks_Extraneous_Data_005()
        {
            // arrange
            var cut = GetReader(Resources.blocks_extraneous_data_005);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Blocks_Extraneous_Data_006()
        {
            // arrange
            var cut = GetReader(Resources.blocks_extraneous_data_006);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Blocks_Extraneous_Data_007()
        {
            // arrange
            var cut = GetReader(Resources.blocks_extraneous_data_007);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Blocks_Overlap_001()
        {
            // arrange
            var cut = GetReader(Resources.blocks_overlap_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<BlockOverlappingException>();
        }

        [Fact]
        public void Invalid_Blocks_Overlap_002()
        {
            // arrange
            var cut = GetReader(Resources.blocks_overlap_002);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<BlockOverlappingException>();
        }

        [Fact]
        public void Invalid_Blocks_Overlap_003()
        {
            // arrange
            var cut = GetReader(Resources.blocks_overlap_003);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<BlockOverlappingException>();
        }

        [Fact]
        public void Invalid_Blocks_Ordering_001()
        {
            // arrange
            var cut = GetReader(Resources.blocks_ordering_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Blocks_Ordering_002()
        {
            // arrange
            var cut = GetReader(Resources.blocks_ordering_002);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Blocks_Ordering_003()
        {
            // arrange
            var cut = GetReader(Resources.blocks_ordering_003);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().WithInnerException<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Blocks_Ordering_004()
        {
            // arrange
            var cut = GetReader(Resources.blocks_ordering_004);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().WithInnerException<InvalidDataException>();
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
        public void Invalid_Blocks_PrivateData_Absent_001()
        {
            // arrange
            var cut = GetReader(Resources.blocks_private_absent_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<InvalidDataException>();
        }

        [Fact]
        public void Invalid_Blocks_Private_001()
        {
            // arrange
            var cut = GetReader(Resources.blocks_private_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().WithInnerException<BlockNotOnBoundaryException>();
        }

        [Fact]
        public void Invalid_Blocks_PrivateData_Absent_002()
        {
            // arrange
            var cut = GetReader(Resources.blocks_private_absent_002);

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
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockNotOnBoundaryException));
        }

        [Fact]
        public void Invalid_Directory_4_Byte_003()
        {
            // arrange
            var cut = GetReader(Resources.directory_4_byte_003);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().ContainItemsAssignableTo<InvalidNullPaddingException>();
        }

        [Fact]
        public void Invalid_Directory_Overlaps_001()
        {
            // arrange
            var cut = GetReader(Resources.directory_overlaps_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockOverlappingException)).And.Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Directory_Overlaps_002()
        {
            // arrange
            var cut = GetReader(Resources.directory_overlaps_002);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().WithInnerException<BlockOverlappingException>();
        }

        [Fact]
        public void Invalid_Directory_Overlaps_003()
        {
            // there should be caught thrown exceptions

            // arrange
            var cut = GetReader(Resources.directory_overlaps_003);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().WithInnerException<BlockMaxPaddingExceededException>();
        }

        [Fact]
        public void Invalid_Directory_Overlaps_004()
        {
            // there should be caught thrown exceptions

            // arrange
            var cut = GetReader(Resources.directory_overlaps_004);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.GetType() == typeof(BlockMaxPaddingExceededException));
        }

        [Fact]
        public void Invalid_Directory_Overlaps_005()
        {
            // arrange
            var cut = GetReader(Resources.directory_overlaps_005);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().WithInnerException<BlockOverlappingException>();
        }

        [Fact]
        public void Invalid_Directory_Extraneous_Data_001()
        {
            // arrange
            var cut = GetReader(Resources.directory_extraneous_data_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().WithInnerException<BlockMaxPaddingExceededException>();
        }

        /*
        [Fact]
        public void Invalid_Directory_CompLength_001()
        {
            // arrange
            var cut = GetReader(Resources.directory_compLength_001);

            // act
            Action action = cut.Process;

            // assert
            action.ShouldThrow<AggregateException>().And.InnerExceptions.Should().Contain(e => e.Message == "The \"hmtx\" table directory entry has a compressed length (22) larger than the original length (16).");
        }*/
    }
}
