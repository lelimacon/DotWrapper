using DotWrapper.Resolve;
using DotWrapper.Utils;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace DotWrapper
{
    /// <summary>
    ///     The Chunk class.
    ///     Represents a piece of data (generally a file).
    /// </summary>
    public class Chunk
    {
        /// <summary>
        ///     Gets or sets the data resolve chain.
        /// </summary>
        public DataResolver ResolveChain { get; set; }

        /// <summary>
        ///     Gets or sets the data name.
        ///     Can be the name of the file (with extension).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the chunk's raw data.
        /// </summary>
        public byte[] Data
        {
            get { return ResolveChain.ClosingResolver.Data; }
            set { ResolveChain.ClosingResolver.Data = value; }
        }

        /// <summary>
        ///     Data constructor (default resolve chain).
        /// </summary>
        public Chunk(string name, byte[] data = null)
            : this(name, DataResolver.DefaultResolveChain(data))
        {
        }

        /// <summary>
        ///     Password constructor (default resolve chain).
        /// </summary>
        public Chunk(string name, string password, byte[] data = null)
            : this(name, DataResolver.DefaultResolveChain(password, data))
        {
        }

        /// <summary>
        ///     Resolve chain constructor.
        /// </summary>
        public Chunk(string name, DataResolver resolveChain)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (resolveChain == null)
                throw new ArgumentNullException("resolveChain");
            Name = name;
            ResolveChain = resolveChain;
        }

        /// <summary>
        ///     Writes the chunk to the stream.
        ///     The chunk data is encrypted with AES.
        ///     Base data is not encrypted in any way.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Number of bytes written.</returns>
        [Pure]
        public int Write(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw new ArgumentException("Invalid stream.");
            byte[] array = ResolveChain.Pack();
            int count = 0;
            count += stream.Write(Name);
            count += stream.Write(array.Length);
            count += stream.Write(array);
            return count;
        }

        /// <summary>
        ///     Reads a chunk from a stream. Password is optional.
        ///     Chunk data is decrypted from AES.
        ///     A new password will be generated as the chunk is created.
        /// </summary>
        /// <param name="stream">The stream in which read the chunk.</param>
        /// <returns>The resulting chunk.</returns>
        [Pure]
        public static Chunk Read(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Invalid stream.");
            string name = stream.ReadString();
            int length = stream.ReadInt32();
            byte[] data = stream.ReadByteArray(length);
            DataResolver resolveChain = DataResolver.Unpack(data);
            Chunk chunk = new Chunk(name, resolveChain);
            return chunk;
        }
    }
}
