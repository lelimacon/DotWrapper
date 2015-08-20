using DotWrapper.Utils;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Converts to and from Base64.
    /// </summary>
    public class Base64Resolver : DataResolver
    {
        internal override IdResolver ClosingResolver
        {
            get { return ChildResolver.ClosingResolver; }
        }

        public Base64Resolver(byte[] data = null)
            : this(new IdResolver(data))
        {
        }

        public Base64Resolver(DataResolver childResolver)
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
                WriteHeader(stream, Resolvers.Base64Resolver);
                string buffer = Convert.ToBase64String(childData);
                stream.Write(buffer, true);
                return stream.ToArray();
            }
        }

        [Pure]
        internal new static Base64Resolver Unpack(byte[] data)
        {
            string base64;
            using (MemoryStream stream = new MemoryStream(data))
                base64 = stream.ReadString();
            byte[] childData = Convert.FromBase64String(base64);
            DataResolver child = DataResolver.Unpack(childData);
            return new Base64Resolver(child);
        }
    }
}
