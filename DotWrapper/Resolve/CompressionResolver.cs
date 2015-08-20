using DotWrapper.Utils;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Compresses the data.
    ///     TODO: Implement this class.
    /// </summary>
    public class CompressionResolver : DataResolver
    {
        internal override IdResolver ClosingResolver
        {
            get { return ChildResolver.ClosingResolver; }
        }

        public CompressionResolver(byte[] data)
            : this(new IdResolver(data))
        {
        }

        public CompressionResolver(DataResolver childResolver)
            : base(childResolver)
        {
            if (childResolver == null)
                throw new ArgumentNullException("childResolver");
        }

        [Pure]
        internal override byte[] Pack()
        {
            byte[] childData = ChildResolver.Pack();
            using (MemoryStream stream = new MemoryStream())
            {
                WriteHeader(stream, Resolvers.CompressionResolver);
                using (var zipStream = new GZipStream(stream, CompressionMode.Compress, true))
                    zipStream.Write(childData);
                return stream.ToArray();
            }
        }

        [Pure]
        internal new static CompressionResolver Unpack(byte[] data)
        {
            byte[] childData;
            using (var inStream = new MemoryStream(data))
            using (var zipStream = new GZipStream(inStream, CompressionMode.Decompress))
            using (var outStream = new MemoryStream())
            {
                zipStream.CopyTo(outStream);
                childData = outStream.ToArray();
            }
            DataResolver child = DataResolver.Unpack(childData);
            return new CompressionResolver(child);
        }
    }
}
