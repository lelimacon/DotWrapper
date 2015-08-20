using DotWrapper.Utils;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace DotWrapper.Resolve
{
    /// <summary>
    ///     Retrieves or writes data from/to a file
    /// </summary>
    public class FileResolver : DataResolver
    {
        public string Path { get; set; }

        internal override IdResolver ClosingResolver
        {
            get { return ChildResolver.ClosingResolver; }
        }

        public FileResolver(string path, byte[] data = null)
            : this(path, new IdResolver(data))
        {
        }

        public FileResolver(string path, DataResolver childResolver)
            : base(childResolver)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            if (childResolver == null)
                throw new ArgumentNullException("childResolver");
            Path = path;
        }

        internal override byte[] Pack()
        {
            // Write to the file.
            byte[] childData = ChildResolver.Pack();
            File.WriteAllBytes(Path, childData);
            // Return resolver properties.
            using (MemoryStream stream = new MemoryStream())
            {
                WriteHeader(stream, Resolvers.FileResolver);
                stream.Write(Path, true);
                return stream.ToArray();
            }
        }

        [Pure]
        internal new static FileResolver Unpack(byte[] data)
        {
            string path;
            using (MemoryStream stream = new MemoryStream(data))
                path = stream.ReadString();
            byte[] childData = File.ReadAllBytes(path);
            DataResolver child = DataResolver.Unpack(childData);
            return new FileResolver(path, child);
        }
    }
}
