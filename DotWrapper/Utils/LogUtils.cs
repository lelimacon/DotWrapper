using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DotWrapper.Utils
{
    static class LogUtils
    {
        public enum Category
        {
            DEBUG, // Debug info that will not be printed on release versions
            INFO, // General info that will be printed on all versions
            WARNING, //
            ERROR, // for severe level problems (will be printed to file + popup)
        }

        private static List<Stream> outputs;

        static LogUtils()
        {
            outputs= new List<Stream>();
        }

        public static void Register(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw new ArgumentException("Invalid stream.");
            outputs.Add(stream);
        }

        /*
        /// <summary>
        /// Writes to log the specified tag and message.
        /// CONSIDER: using Trace.Listeners (it displays Excel-DNA Trace however).
        /// </summary>
        public static void Write(this Category category, string tag, string message = null)
        {
            string line = BuildEntry(category, tag, message);
            //using (StreamWriter writer = new StreamWriter(LOG_FILE, true))
            //{
            //    writer.WriteLine(line);
            //}
            Trace.Listeners.Add()
            Trace.WriteLine(line);
            Trace.Flush();
            foreach (Stream stream in outputs)
            {
            }
        }
        */

        public static string BuildEntry(this Category category, string tag, string message = null)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (message == null)
                return String.Format("{0} | {1} | {2}", now, category, tag);
            return String.Format("{0} | {1} | {2} | {3}", now, category, tag, message);
        }

        /// <summary>
        /// Create a log tag.
        /// </summary>
        public static string Tag(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            return GetTag(memberName, sourceFilePath, sourceLineNumber);
        }

        private static string GetTag(string name, string path, int line)
        {
            path = path.Substring(path.LastIndexOf('\\') + 1);
            return String.Format("{0}({1}:{2})", name, path, line);
        }

    }
}
