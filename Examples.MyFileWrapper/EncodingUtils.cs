using System;
using System.IO;
using System.Text;

namespace MyFileWrapper
{
    internal static class EncodingUtils
    {
        /// <summary>
        ///     Transforms a byte array to string after determining its encoding.
        /// </summary>
        /// <param name="data">The data to analyse.</param>
        /// <returns>The resulting string.</returns>
        public static string Decode(this byte[] data)
        {
            // Easy and accurate way of decoding (does not detect UTF7.
            return new StreamReader(new MemoryStream(data), true).ReadToEnd();
            //return data.GetEncoding().GetString(data);
        }

        /// <summary>
        ///     Determines a text file's encoding by analyzing its byte order mark (BOM).
        ///     Defaults to ANSI when detection of the text file's endianness fails.
        /// </summary>
        /// <param name="filePath">The file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filePath)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                file.Read(bom, 0, 4);
            return bom.GetEncoding();
            // An alternative would be using a StreamReader to detect it.
            //using (StreamReader reader = new StreamReader(filePath, Encoding.Default, true))
            //{
            //    reader.Peek();
            //    return reader.CurrentEncoding;
            //}
        }

        /// <summary>
        ///     Determines a text's encoding by analyzing its byte order mark (BOM).
        ///     Defaults to ANSI when detection of the text's endianness fails.
        /// </summary>
        /// <param name="b">The text buffer.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(this byte[] b)
        {
            if (b != null && b.Length >= 4)
                throw new ArgumentNullException("b");
            if (b[0] == 0xff && b[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (b[0] == 0xfe && b[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) return Encoding.UTF7;
            if (b[0] == 0xef && b[1] == 0xbb && b[2] == 0xbf) return Encoding.UTF8;
            if (b[0] == 0 && b[1] == 0 && b[2] == 0xfe && b[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default; // OS's current ANSI code page.
        }
    }
}
