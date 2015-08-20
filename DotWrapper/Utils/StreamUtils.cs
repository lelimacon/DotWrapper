using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DotWrapper.Utils
{
    internal static class StreamUtils
    {
        #region READ

        /// <summary>
        ///     Reads a byte array from a stream.
        /// </summary>
        /// <param name="s">The stream (must be valid).</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>The read and transformed byte.</returns>
        public static byte ReadByte(this Stream s, ByteTransform t)
        {
            Debug.Assert(s != null && s.CanRead);
            int i = s.ReadByte();
            if (i == -1)
                throw new ArgumentException("End Of Stream");
            byte b = (byte) i;
            return b.Transform(t);
        }

        /// <summary>
        ///     Reads an int of 4 bytes from a stream.
        /// </summary>
        /// <param name="s">The stream (must be valid).</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>The read int.</returns>
        public static int ReadInt32(this Stream s,
            ByteTransform t = ByteTransform.None)
        {
            byte[] bytes = {s.ReadByte(t), s.ReadByte(t), s.ReadByte(t), s.ReadByte(t)};
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        ///     Reads a stream and fills a string until the byte 0 or end of stream.
        /// </summary>
        /// <param name="s">The stream (must be valid).</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>The read string.</returns>
        public static string ReadString(this Stream s,
            ByteTransform t = ByteTransform.None)
        {
            string res = string.Empty;
            int i;
            byte b;
            while ((i = s.ReadByte()) != -1 && (b = ((byte) i).Transform(t)) != 0)
                res += (char) b;
            return res;
        }

        /// <summary>
        ///     Reads a stream and fills a byte array until the end of stream.
        /// </summary>
        /// <param name="s">The stream (must be valid).</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>The read byte array.</returns>
        public static byte[] ReadByteArray(this Stream s,
            ByteTransform t = ByteTransform.None)
        {
            List<byte> res = new List<byte>();
            int i;
            byte b;
            while ((i = s.ReadByte()) != -1 && (b = ((byte) i).Transform(t)) != 0)
                res.Add(b);
            return res.ToArray();
        }

        /// <summary>
        ///     Reads a stream and fills a byte array until the byte 0 or end of stream.
        /// </summary>
        /// <param name="s">The stream (must be valid).</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>The read byte array.</returns>
        public static byte[] ReadToEnd(this Stream s,
            ByteTransform t = ByteTransform.None)
        {
            List<byte> res = new List<byte>();
            int i;
            while ((i = s.ReadByte()) != -1)
                res.Add(((byte)i).Transform(t));
            return res.ToArray();
        }

        /// <summary>
        ///     Reads a byte array from a stream.
        /// </summary>
        /// <param name="s">The stream (must be valid).</param>
        /// <param name="size">Size of the array.</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>The read byte array.</returns>
        public static byte[] ReadByteArray(this Stream s, int size,
            ByteTransform t = ByteTransform.None)
        {
            Debug.Assert(s != null && s.CanRead);
            byte[] buffer = new byte[size];
            if (t == ByteTransform.None)
            {
                // Beware: when encrypting size can vary (only without byte transform).
                int l = s.Read(buffer, 0, size);
                if (l != size)
                    buffer = buffer.Subset(0, l);
            }
            else
                for (int i = 0; i < size; i++)
                    buffer[i] = s.ReadByte(t);
            return buffer;
        }

        #endregion READ

        #region WRITE

        /// <summary>
        ///     Writes byte to a stream.
        /// </summary>
        /// <param name="stream">The stream (must be valid).</param>
        /// <param name="value">The value to write.</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Write(this Stream stream, byte value, ByteTransform t)
        {
            Debug.Assert(stream != null && stream.CanWrite);
            stream.WriteByte(value.Transform(t));
            return 1;
        }

        /// <summary>
        ///     Writes an integer (theoredically 4 bytes) to a stream.
        /// </summary>
        /// <param name="stream">The stream (must be valid).</param>
        /// <param name="value">The value to write.</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Write(this Stream stream, int value,
            ByteTransform t = ByteTransform.None)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            foreach (var b in bytes)
                stream.Write(b, t);
            return bytes.Length;
        }

        /// <summary>
        ///     Writes a byte array to a stream.
        /// </summary>
        /// <param name="stream">The stream (must be valid).</param>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="knownSize">If true char 0 will be added after the buffer.</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Write(this Stream stream, byte[] buffer, bool knownSize = true,
            ByteTransform t = ByteTransform.None)
        {
            Debug.Assert(stream != null && stream.CanWrite);
            buffer = buffer ?? new byte[] {};
            if (t == ByteTransform.None)
                stream.Write(buffer, 0, buffer.Length);
            else
                foreach (var b in buffer)
                    stream.Write(b, t);
            if (!knownSize)
                stream.WriteByte(0);
            return (buffer.Length + (knownSize ? 0 : 1));
        }

        /// <summary>
        ///     Writes a string followed by the char 0 to a stream (if eos).
        /// </summary>
        /// <param name="stream">The stream (must be valid).</param>
        /// <param name="buffer">The buffer to write.</param>
        /// <param name="eos">After this string will be the End Of Stream.</param>
        /// <param name="t">The transform to apply to each byte.</param>
        /// <returns>Number of bytes written.</returns>
        public static int Write(this Stream stream, string buffer, bool eos = false,
            ByteTransform t = ByteTransform.None)
        {
            buffer = buffer ?? string.Empty;
            foreach (char c in buffer)
                stream.Write((byte) c, t);
            if (!eos)
                stream.Write((byte) 0, t);
            return buffer.Length + 1;
        }

        #endregion WRITE
    }
}
