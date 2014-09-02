using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Commons.Xml.Relaxng;

using Ionic.Zlib;

using WoffDotNet.Exceptions;
using WoffDotNet.Properties;
using WoffDotNet.Types;
using WoffDotNet.Validators;

namespace WoffDotNet.Readers
{
    public class WoffMetadataReader
    {
        private readonly byte[] _bytes;

        private readonly WoffHeader _header;

        public WoffMetadataReader(byte[] bytes, WoffHeader header)
        {
            Contract.Requires(bytes != null);
            Contract.Requires(header != null);

            _bytes = bytes;
            _header = header;
        }

        public void Process()
        {
            var exceptions = new List<Exception>();
            var metaBytes = _bytes;
            var hasInvalidLengths = false;
            if (_header.MetaLength <= _header.MetaOrigLength)
            {
                try
                {
                    metaBytes = ZlibStream.UncompressBuffer(_bytes);
                    hasInvalidLengths = !WoffMetadataValidator.ValidateLengths(_header.MetaOrigLength, (uint)metaBytes.Length);
                }
                catch (ZlibException e)
                {
                    exceptions.Add(new WoffUncompressException("Cannot uncompress metadata", e));
                    Exceptions = exceptions;
                    return;
                }
            }

            if (hasInvalidLengths)
            {
                exceptions.Add(new InvalidRangeException("The stated metadata length is not equal to the actual value"));
            }
            else
            {
                var xmlDocument = new XmlDocument();
                var encoding = metaBytes.GetEncoding();
                if (Equals(encoding, Encoding.UTF8))
                {
                    var memoryStream = new MemoryStream(metaBytes);
                    Stream validationStream = new MemoryStream(Resources.woffmeta_rng);
                    using (var xmlValidationReader = XmlReader.Create(validationStream))
                    using (var xmlReader = XmlReader.Create(memoryStream))
                    using (var validatingReader = new RelaxngValidatingReader(xmlReader, xmlValidationReader))
                    {
                        validatingReader.InvalidNodeFound += (source, message) =>
                        {
                            exceptions.Add(new InvalidWoffMetadaElementException(message));
                            return true;
                        };

                        xmlDocument.Load(validatingReader);

                        if (!xmlDocument.HasEncoding("UTF-8"))
                        {
                            exceptions.Add(new EncodingNotSupportedException("metadata must be encoded with UTF-8"));
                        }
                    }

                    var aggregateException = xmlDocument.ValidateWoffMetadata();
                    if (aggregateException != null)
                    {
                        exceptions.AddRange(aggregateException.InnerExceptions);
                    }
                    else
                    {
                        Document = xmlDocument;
                    }
                }
                else
                {
                    exceptions.Add(new EncodingNotSupportedException("Found BOM, metadata must be encoded with UTF-8"));
                }
            }

            if (exceptions.Any())
            {
                Exceptions = exceptions;
                Document = null;
            }
        }

        public XmlDocument Document { get; private set; }

        public IEnumerable<Exception> Exceptions { get; private set; }
    }
}
