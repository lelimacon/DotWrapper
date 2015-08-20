using DotWrapper.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DotWrapper
{
    /// <summary>
    ///     The wrap class.
    ///     Represents a set of chunks in a file.
    ///     Provides a set of static "properties" that the user can edit.
    /// </summary>
    public class Wrap
    {
        #region Static "properties"

        /// <summary>
        ///     The specific set of bytes that defines a wrap.
        /// </summary>
        public static byte[] Signature = {0, (byte) '=', 0};

        #endregion Static "properties"

        /// <summary>
        ///     Gets or sets the base file data.
        ///     Not encrypted.
        /// </summary>
        public byte[] BaseData { get; set; }

        /// <summary>
        ///     Gets or sets a Chunk represents a file data.
        ///     Added after the base data.
        /// </summary>
        public List<Chunk> Chunks { get; set; }

        /// <summary>
        ///     Constructor.
        ///     Can be called without any arguments for an empty wrap.
        /// </summary>
        /// <param name="baseData">Unchanged data at the beginning of the file. Will default to an empty array.</param>
        /// <param name="chunks">The chunks stored in the wrap. Will default to an empty list.</param>
        public Wrap(byte[] baseData = null, List<Chunk> chunks = null)
        {
            BaseData = baseData ?? new byte[] {};
            Chunks = chunks ?? new List<Chunk>();
        }

        /// <summary>
        ///     Finds the first chunk matching the specified name.
        ///     Beware that chunks with the same name can hide each other.
        ///     Manually search the Chunks list for a specific search.
        /// </summary>
        /// <param name="name">Name of the chunk to search for.</param>
        /// <returns>The first chunk with the given name, or throws an ArgumentException.</returns>
        public Chunk GetChunk(string name)
        {
            Chunk chunk = Chunks.Find(c => c.Name == name);
            if (chunk == null)
                throw new ArgumentException("Unable to find chunk " + name);
            return chunk;
        }

        #region WRITE

        /// <summary>
        ///     Creates a file for this wrap (whipes file original data).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [Pure]
        public string Write(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                Write(stream);
            return path;
        }

        /// <summary>
        ///     Writes this wrap to the stream.
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
            int count = 0;
            count += stream.Write(BaseData);
            count += Chunks.Sum(chunk => chunk.Write(stream));
            count += stream.Write(BaseData.Length);
            count += stream.Write(Chunks.Count);
            count += stream.Write(Signature);
            return count;
        }

        #endregion WRITE

        #region READ

        /// <summary>
        ///     Reads the executable running this code as a wrap.
        /// </summary>
        /// <returns>This wrap.</returns>
        [Pure]
        public static Wrap This()
        {
            string exeLoc = Assembly.GetEntryAssembly().Location;
            return Read(exeLoc, true);
        }

        /// <summary>
        ///     Reads a Wrap from a file.
        /// </summary>
        /// <param name="path">The path from which to read the wrap.</param>
        /// <param name="create">Create the wrap with the base data if none.</param>
        /// <returns>The resulting wrap.</returns>
        [Pure]
        public static Wrap Read(string path, bool create = true)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            Wrap wrap;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                wrap = Read(stream);
            if (wrap == null && create)
            {
                byte[] data = File.ReadAllBytes(path);
                wrap = new Wrap(data);
            }
            return wrap;
        }

        /// <summary>
        ///     Reads a Wrap from a stream.
        /// </summary>
        /// <param name="stream">The stream from which to read the wrap.</param>
        /// <returns>The resulting wrap or null if signature is not found.</returns>
        public static Wrap Read(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Invalid stream.");
            if (!Valid(stream))
                return null;
            stream.Seek(-Signature.Length - 2 * sizeof (int), SeekOrigin.End);
            int offset = stream.ReadInt32();
            int chunksNumber = stream.ReadInt32();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] data = stream.ReadByteArray(offset);
            List<Chunk> chunks = new List<Chunk>();
            for (int i = 0; i < chunksNumber; i++)
                chunks.Add(Chunk.Read(stream));
            return new Wrap(data, chunks);
        }

        /// <summary>
        ///     Checks if a file has a valid wrap signature.
        ///     Starts reading at the end of the file.
        /// </summary>
        /// <param name="path">The path from which to read the wrap.</param>
        /// <returns>True if the signature matches, false otherwise.</returns>
        [Pure]
        public static bool Valid(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return Valid(stream);
        }

        /// <summary>
        ///     Checks if a stream has a wrap valid signature.
        ///     Reads at the end of the stream.
        /// </summary>
        /// <param name="stream">The stream to check on.</param>
        /// <returns>True if the signature matches, false otherwise.</returns>
        public static bool Valid(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Invalid stream.");
            stream.Seek(-Signature.Length, SeekOrigin.End);
            return !(from b in Signature where stream.ReadByte() != b select b).Any();
        }

        #endregion READ
    }
}
