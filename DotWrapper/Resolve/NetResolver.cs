using DotWrapper.Utils;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Will download a data from the net.
    ///     /!\ This resolver is currently one-way only (unpacking) /!\
    ///     TODO: Consider adding a callback for packing.
    /// </summary>
    public class NetResolver : DataResolver
    {
        /// <summary>
        ///     TODO: Consider using Uri instead.
        /// </summary>
        public string Address { get; set; }

        internal override IdResolver ClosingResolver
        {
            get { return ChildResolver.ClosingResolver; }
        }

        public NetResolver(string address, byte[] data = null)
            : this(address, new IdResolver(data))
        {
        }

        public NetResolver(string address, DataResolver childResolver)
            : base(childResolver)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException("address");
            if (childResolver == null)
                throw new ArgumentNullException("childResolver");
            Address = address;
        }

        [Pure]
        internal override byte[] Pack()
        {
            // /!\ One cannot edit the source data from a NetResolver /!\
            // Return resolver properties.
            using (MemoryStream stream = new MemoryStream())
            {
                WriteHeader(stream, Resolvers.NetResolver);
                stream.Write(Address, true);
                return stream.ToArray();
            }
        }

        [Pure]
        internal new static NetResolver Unpack(byte[] data)
        {
            string address;
            using (MemoryStream stream = new MemoryStream(data))
                address = stream.ReadString();
            byte[] childData = new WebClient().DownloadData(address);
            DataResolver child = DataResolver.Unpack(childData);
            return new NetResolver(address, child);
        }
    }
}
