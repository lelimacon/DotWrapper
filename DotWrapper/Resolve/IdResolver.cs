using DotWrapper.Utils;
using System.Diagnostics.Contracts;
using System.IO;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Closing resolver for resolve chain.
    /// </summary>
    public class IdResolver : DataResolver
    {
        private byte[] _data;

        public byte[] Data
        {
            get { return _data; }
            // Prevent the data from being null. Null is evil.
            set { _data = value ?? new byte[] {}; }
        }

        internal override IdResolver ClosingResolver
        {
            get { return this; }
        }

        public IdResolver(byte[] data = null)
            : base(null)
        {
            Data = data;
        }

        [Pure]
        internal override byte[] Pack()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteHeader(stream, Resolvers.IdResolver);
                stream.Write(Data);
                return stream.ToArray();
            }
        }

        [Pure]
        internal new static IdResolver Unpack(byte[] data)
        {
            return new IdResolver(data);
        }
    }
}
