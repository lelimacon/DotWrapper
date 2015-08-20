using DotWrapper.Utils;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Reference of all resolvers.
    ///     Do NOT change order at risk of incompatibility.
    /// </summary>
    internal enum Resolvers : byte
    {
        IdResolver = 0,
        CryptoResolver = 1,
        CompressionResolver = 2,
        NetResolver = 3,
        Base64Resolver = 4,
        FileResolver = 5
        //XmlResolver,
        //ExecutionResolver,
        //SteganoResolver,
        //OcrResolver,
    }

    /// <summary>
    ///     Resolvers parent.
    /// </summary>
    public abstract class DataResolver
    {
        public DataResolver ChildResolver { get; set; }

        public ByteTransform PropertyTransform { get; set; }

        internal abstract IdResolver ClosingResolver { get; }

        /// <summary>
        ///     Base constructor.
        ///     TODO: ByteTransform
        /// </summary>
        /// <param name="childResolver"></param>
        protected DataResolver(DataResolver childResolver)
        {
            // No null check for childResolver (can be a closing resolver).
            ChildResolver = childResolver;
        }

        /// <summary>
        ///     Parent method for resolver packing.
        ///     Retrieves the unpacked (resolved) data from the closing resolver.
        /// </summary>
        /// <returns>The packed data (raw).</returns>
        internal abstract byte[] Pack();

        /// <summary>
        ///     Parent function for resolver unpacking.
        /// </summary>
        /// <param name="data">The packed data (raw).</param>
        /// <returns>The unpacked data (resolved).</returns>
        [Pure]
        internal static DataResolver Unpack(byte[] data)
        {
            if (data.Length <= 2 || data[0] != '~')
                return IdResolver.Unpack(data);
            byte resolverType = (byte) (char) data[1];
            byte[] childData = data.Subset(2);
            switch ((Resolvers) (resolverType - '0'))
            {
                case Resolvers.IdResolver:
                    return IdResolver.Unpack(childData);

                case Resolvers.CryptoResolver:
                    return CryptoResolver.Unpack(childData);

                case Resolvers.CompressionResolver:
                    return CompressionResolver.Unpack(childData);

                case Resolvers.NetResolver:
                    return NetResolver.Unpack(childData);

                case Resolvers.Base64Resolver:
                    return Base64Resolver.Unpack(childData);

                case Resolvers.FileResolver:
                    return FileResolver.Unpack(childData);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        internal static void WriteHeader(MemoryStream stream, Resolvers resolver)
        {
            stream.WriteByte((byte) '~');
            stream.WriteByte((byte) ((byte) resolver + '0'));
        }

        internal static DataResolver DefaultResolveChain(byte[] data)
        {
            return new CryptoResolver(new CompressionResolver(data));
        }

        internal static DataResolver DefaultResolveChain(string password, byte[] data = null)
        {
            return new CryptoResolver(new CompressionResolver(data), password);
        }
    }
}
