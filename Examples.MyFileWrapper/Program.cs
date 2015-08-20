using DotWrapper;
using DotWrapper.Arguments;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace MyFileWrapper
{
    internal class Program
    {
        public static bool Silent { get; set; }

        private static void Main(string[] args)
        {
            Silent = false;
            //Wrap.Salt = GenerateSalt(DateTime.Now);
#if DEBUG
            if (args.Length == 0)
            {
                // Simple wrap testing.
                WrapProcessor processor = new WrapProcessor(Wrap.This(), false);
                processor.WrapIn("Program.cs", "test.cs", null, false);
                //processor.WrapIn("MyFileWrapper.exe.config", "test.xml", null, false);
                processor.Wrap = Wrap.Read("MyFileWrapper-new.exe");
                processor.Dump(null); // Dump everything
                Console.WriteLine("Done.");
                Console.ReadLine();
                return;
            }
#endif
            ProcessArguments(args, 2);
        }

        /// <summary>
        ///     Generate a pseudo-random byte array using the file creation/modification date.
        /// </summary>
        /// <param name="date">the date and time to generate salt to</param>
        /// <returns></returns>
        private static byte[] GenerateSalt(DateTime date)
        {
            return Encoding.ASCII.GetBytes("sAl7I$n07aLie");
        }

        #region Argument Processor

        public static int ProcessArguments(string[] args, int startIndex)
        {
            WrapProcessor processor = new WrapProcessor(Wrap.This(), false);
            if (args.Length <= 0)
                return Usage(1);
            // Try to detect desperate calls for help.
            if (args.Length == 1 && (args[0] == "-h" || args[0] == "help" || args[0] == "-help"))
                return Usage(0);
            // Try to detect version info.
            if (args.Length == 1 && (args[0] == "-v" || args[0] == "version" || args[0] == "-version"))
                return Version(0);
            // Create options
            Option on = new Option(true, 'n');
            Option oo = new Option(true, 'o');
            Option oE = new Option(false, 'E');
            Option oX = new Option(false, 'X');
            Option oL = new Option(false, 'L');
            string command = args[0];
            // Process commands.
            try
            {
                Option[] os;
                switch (command)
                {
                    case "wrap":
                        os = new[] {on, oo, oE, oX, Option.NoName()};
                        os.CrawlArgs(args, 1);
                        processor.WrapIn(os.Arg((char) 1), on.Arg, oo.Arg, oX.Specified);
                        break;

                    case "remove":
                        os = new[] {on, oo};
                        os.CrawlArgs(args, 1);
                        processor.Remove(on.Arg, oo.Arg);
                        break;

                    // TODO: determine the arguments (change name & password)
                    case "edit":
                        os = new[] {on, oo};
                        os.CrawlArgs(args, 1);
                        processor.Edit(on.Arg, oo.Arg);
                        break;

                    case "dump":
                        os = new[] {on, oo, oE, oX, oL};
                        os.CrawlArgs(args, 1);
                        if (oL.Specified && (oo.Arg != null || oX.Specified))
                            throw new ArgumentException("Incompatible options");
                        if (oL.Specified)
                            processor.Dump(on.Arg);
                        else
                            processor.Dump(on.Arg, oo.Arg, oX.Specified);
                        break;

                    case "info":
                        os = new[] {on};
                        os.CrawlArgs(args, 1);
                        processor.Info(on.Arg);
                        break;

                    default:
                        return Usage(1);
                }
            }
                // Considered as a user error.
            catch (ArgumentException e)
            {
#if DEBUG
                Console.Error.WriteLine(e);
#else
                if (!Silent)
                    Console.Error.WriteLine(e.Message);
#endif
                return Usage();
            }
                // Considered as an internal error.
            catch (Exception e)
            {
                if (!Silent)
                    Console.Error.WriteLine(e);
            }
            return 0;
        }

        #endregion Argument Processor

        #region Usage & Version

        /// <summary>
        ///     Displays the usage to console output.
        /// </summary>
        /// <param name="errCode">return code</param>
        /// <returns>errCode</returns>
        private static int Usage(int errCode = 0)
        {
            if (Silent)
                return errCode;
            TextWriter stream = errCode == 0 ? Console.Out : Console.Error;
            stream.WriteLine("Usage : wrapper.exe CMD OPTS");
            stream.WriteLine("");
            stream.WriteLine("Commands");
            stream.WriteLine("    wrap    [-n NAME] [-o PATH] [-p PASS | -X] [-E] PATH");
            stream.WriteLine("    remove  [-n NAME] [-o PATH]");
            stream.WriteLine("    edit    [-n NAME] [-o PATH] [-p PASS]");
            stream.WriteLine("    dump    [-n NAME] [-E -o PATH | -L] [-p PASS | -X]");
            stream.WriteLine("    info    [-n NAME]");
            stream.WriteLine("");
            stream.WriteLine("Options");
            stream.WriteLine("    -n Specify a name.");
            stream.WriteLine("       If not specified, command wrap will choose the file name.");
            stream.WriteLine("       The other commands will run on all files.");
            stream.WriteLine("    -o Specify output folder or path whith -n.");
            stream.WriteLine("       If not specified, a default file name will be set.");
            stream.WriteLine("    -p Specify encryption/decryption key.");
            stream.WriteLine("    -E Execute executable content or mark data as executable.");
            stream.WriteLine("    -L Dump to console.");
            stream.WriteLine("    -X Do not encrypt/decrypt.");
            return errCode;
        }

        /// <summary>
        ///     Displays the usage to console output.
        /// </summary>
        /// <param name="errCode">return code</param>
        /// <returns>errCode</returns>
        private static int Version(int errCode = 0)
        {
            if (Silent)
                return errCode;
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            TextWriter stream = errCode == 0 ? Console.Out : Console.Error;
            stream.WriteLine("MyfileWrapper version " + version);
            stream.WriteLine("Example using the DotWrapper library.");
            return errCode;
        }

        #endregion Usage & Version
    }
}
