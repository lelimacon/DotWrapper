using System;
using System.Linq;

namespace DotWrapper.Arguments
{
    /// <summary>
    /// Option class, can be used to check and retrieve the program arguments.
    /// </summary>
    public class Option
    {
        /// <summary>
        /// The short name of the argument (one character).
        /// </summary>
        public char Name { get; set; }

        /// <summary>
        /// Decides whether the option awaits an argument or not.
        /// </summary>
        public bool HasArg { get; private set; }

        /// <summary>
        /// This value will be set by the argument crawling algorithm.
        /// True if the option is called (with or without an argument).
        /// </summary>
        public bool Specified { get; set; }

        /// <summary>
        /// This value will be set by the argument crawling algorithm.
        /// Contains the argument, if any, or null.
        /// </summary>
        public string Arg { get; set; }

        public Option(bool hasArg, char name)
        {
            HasArg = hasArg;
            Name = name;
            Specified = false;
            Arg = null;
        }

        /// <summary>
        /// Create an Option with no name, typically just a command or string.
        /// </summary
        public static Option NoName()
        {
            return new Option(false, (char) 0);
        }
    }

    public static class Utils
    {
        /// <summary>
        ///     Fill the options from the program arguments.
        /// </summary>
        /// <param name="options">the options</param>
        /// <param name="args">the program arguments</param>
        /// <param name="startIndex">the first startIndex arguments will be ignored</param>
        public static void CrawlArgs(this Option[] options, string[] args, int startIndex = 0)
        {
            try
            {
                char noArgCount = (char) 0;
                // Iterate throw all arguments.
                for (int i = startIndex; i < args.Length; i++)
                {
                    string arg = args[i];
                    // Argument has a name.
                    if (arg.Length == 2 && arg.StartsWith("-"))
                    {
                        Option opt = options.Get(arg[1]);
                        if (opt.HasArg)
                            opt.Arg = args[++i];
                        opt.Specified = true;
                    }
                    // No-name option.
                    else
                    {
                        Option opt = options.Get((char) 0);
                        opt.Name = ++noArgCount;
                        opt.Specified = true;
                        opt.Arg = arg;
                    }
                }
                // Integrety check.
                if (options.Any(o => o.Name == (char) 0 && !o.Specified))
                    throw new ArgumentException("Inconsistent arguments");
            }
            catch (Exception)
            {
                throw new ArgumentException("Inconsistent arguments");
            }
        }

        public static Option Get(this Option[] options, char name)
        {
            return options.First(o => name == o.Name);
        }

        /// <summary>
        /// Gets the argument with no name with the given index.
        /// </summary>
        /// <param name="options">the options array</param>
        /// <param name="index">index of the no-name argument</param>
        /// <returns>the option, can throw an ArgumentException</returns>s
        public static Option NoName(this Option[] options, int index)
        {
            return options.First(o => (char) index == o.Name);
        }

        /// <summary>
        /// Accessor for the "arg" Option property.
        /// </summary>
        public static string Arg(this Option[] options, char name)
        {
            return options.Get(name).Arg;
        }

        /// <summary>
        /// Accessor for the "specified" Option property.
        /// </summary>
        /// <returns></returns>
        public static bool Spec(this Option[] options, char name)
        {
            return options.Get(name).Specified;
        }
    }
}
