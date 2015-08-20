using System;
using System.IO;
using System.Linq;
using System.Reflection;
using DotWrapper;

namespace MyFileWrapper
{
    public class WrapProcessor
    {
        public bool Silent { get; set; }

        public Wrap Wrap { get; set; }

        public WrapProcessor(Wrap wrap, bool silent)
        {
            Wrap = wrap;
            Silent = silent;
        }

        #region Commands

        public void WrapIn(string inPath, string name, string outPath, bool executable)
        {
            name = name ?? new FileInfo(inPath).Name;
            outPath = outPath ?? NewOutputPath(Assembly.GetExecutingAssembly().Location);
            byte[] data = File.ReadAllBytes(inPath);
            Wrap.Chunks.Add(new Chunk(name, data));
            Wrap.Write(outPath);
            if (!Silent)
                Console.WriteLine("Wrapped \"{0}\" to file \"{1}\".", name, outPath);
        }

        public void Remove(string outPath)
        {
            outPath = outPath ?? NewOutputPath(Assembly.GetExecutingAssembly().Location);
            Wrap.Chunks.Clear();
            Wrap.Write(outPath);
            if (!Silent)
                Console.WriteLine("Removed all data in file \"{0}\".", outPath);
        }

        public void Remove(string name, string outPath)
        {
            if (string.IsNullOrEmpty(name))
            {
                Remove(outPath);
                return;
            }
            outPath = outPath ?? NewOutputPath(Assembly.GetExecutingAssembly().Location);
            Chunk chunk = Wrap.GetChunk(name);
            Wrap.Chunks.Remove(chunk);
            Wrap.Write(outPath);
            if (!Silent)
                Console.WriteLine("Removed \"{0}\" in file \"{1}\".", chunk.Name, outPath);
        }

        /// <summary>
        ///     Simply adds "-new" suffix to the file name.
        /// </summary>
        /// <param name="path">path to the executable file</param>
        /// <returns>path with the new file name</returns>
        private static string NewOutputPath(string path)
        {
            FileInfo info = new FileInfo(path);
            string dir = info.Directory != null ? info.Directory.FullName : string.Empty;
            string name = info.Name.Remove(info.Name.LastIndexOf('.'));
            return Path.Combine(dir, name + "-new" + info.Extension);
        }

        public void Rename(string outPath, string oldname, string newname)
        {
            outPath = outPath ?? NewOutputPath(Assembly.GetExecutingAssembly().Location);
            Wrap.GetChunk(oldname).Name = newname;
            Wrap.Write(outPath);
            if (!Silent)
                Console.WriteLine("Renamed \"{0}\" to \"{1}\".", oldname, newname);
        }

        public void Dump(string name, string outPath, bool execute)
        {
            if (name == null)
            {
                foreach (Chunk chunk in Wrap.Chunks)
                {
                    outPath = outPath ?? string.Empty;
                    string path = Path.Combine(outPath, chunk.Name);
                    Dump(chunk, path, execute);
                }
            }
            else
            {
                Chunk chunk = Wrap.GetChunk(name);
                Dump(chunk, outPath, execute);
            }
        }

        public void Dump(Chunk chunk, string outPath, bool execute)
        {
            outPath = outPath ?? chunk.Name;
            File.WriteAllBytes(outPath, chunk.Data);
            if (!Silent)
                Console.WriteLine("Dumped \"{0}\" to file \"{1}\".", chunk.Name, outPath);
        }

        public void Dump(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                foreach (Chunk chunk in Wrap.Chunks)
                {
                    Console.WriteLine("----\n{0}\n----", chunk.Name);
                    string buffer = chunk.Data.Decode();
                    Console.WriteLine(buffer);
                }
            }
            else
            {
                Chunk chunk = Wrap.GetChunk(name);
                Console.WriteLine("----\n{0}\n----", chunk.Name);
                string buffer = chunk.Data.Decode();
                Console.WriteLine(buffer);
            }
        }

        public void Info(string name)
        {
            if (name == null)
            {
                Console.WriteLine("There are {0} chunk(s) for a total of {1} bytes.",
                    Wrap.Chunks.Count, Wrap.Chunks.Sum(c => c.Data.Length));
                foreach (Chunk chunk in Wrap.Chunks)
                    Console.WriteLine("    {0} [{1} bytes]", chunk.Name, chunk.Data.Length);
            }
            else
            {
                Chunk chunk = Wrap.Chunks.Find(c => c.Name == name);
                if (chunk == null)
                    throw new ArgumentException("Unable to find chunk " + name);
                Console.WriteLine("Chunk Name = {0}", chunk.Name);
                Console.WriteLine("Chunk Size = {0} bytes", chunk.Data.Length);
            }
        }

        #endregion Commands
    }
}
