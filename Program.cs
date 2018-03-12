using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesBlaster
{
    class Program
    {
        static void Main(string[] args)
        {

            var arguments = argsHandler(args);
            printInfo();

            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Red;

            var command = action("-a", arguments);
            var asyncExtraction = action("-p", arguments)=="async";
            var source = action("-s", arguments);
            var target = action("-t", arguments);
            var fileSize = action("-fs", arguments);
            var ignoreWhiteSpace = action("-i", arguments)=="ignore";
            bool combine = (command=="fuse");
            var LogDir = new DirectoryInfo(target + @"\log");


            var CombineSourceDirectory = new DirectoryInfo(source);
            var CombineTargetDir = new DirectoryInfo(target);

            //var CombineSourceDirectory = new DirectoryInfo(@"E:\playground\funny\angular2-webpack-starter");
            //var CombineTargetDir = new DirectoryInfo(@"e:\dumpi\g");


            //var SplitSourceDir = new DirectoryInfo(@"e:\dumpi\g");
            //var SplitTargetDir = new DirectoryInfo(@"e:\dumpi\g\d");

            if (String.IsNullOrEmpty(command)) {
                Console.Write("missing argument: command is missing [fuse or extract].");
                Environment.Exit(0);
            }

            if (String.IsNullOrEmpty(source) || String.IsNullOrEmpty(target)) {
                Console.Write("missing argument: source folder or target folder is missing");
                Environment.Exit(0);
            }
                var SplitSourceDir = new DirectoryInfo(source);
            var SplitTargetDir = new DirectoryInfo(target);

            //var SplitSourceDir = new DirectoryInfo(@"E:\blaster_app_1\app");

            if (combine)
            {
                List<BlasterFilter> binaryFilesFilter = new List<BlasterFilter>()
                {
                    new BlasterFilter()
                };
                var blaster = new Blaster(
                    new BlasterFuse(CombineSourceDirectory, CombineTargetDir, LogDir, false));

                blaster.Init(binaryFilesFilter);
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Done fusing files!");
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                //var blaster = new Blaster(SourceDirectory, CombineTargetDir, LogDir,true);
                //blaster.Combine(new BlasterFilter());
            }
            else {

                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (asyncExtraction)
                {
                    var blaster = new Blaster(
                        new BlasterExtractParallelTaskManager(SplitSourceDir, SplitTargetDir, LogDir, false));
                    blaster.Init();
                }
                else {
                    var blaster = new Blaster(
                     new BlasterExtract(SplitSourceDir, SplitTargetDir, LogDir, false));
                    blaster.Init();
                }


                
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Done extracting files!");

                sw.Stop();
                Console.WriteLine(sw.Elapsed);

                //var blaster = new Blaster(SplitSourceDir, SplitTargetDir, LogDir, false);
                //blaster.Split();
            }

            Console.ReadLine();
        }

        static string action(string name, Dictionary<string, string> arguments) {
            if (!arguments.ContainsKey(name))
            {
                if (name == "-p")
                {
                    return "async";
                }
                else if (name == "-i")
                {
                    return "ignore";
                }
                else if (name == "-fs") {
                    return "50000000";
                }
                else
                {
                    printInfo();
                    Environment.Exit(0);
                    return "";
                }

            }
            else {
                return arguments[name];
            }
        }
        static void printInfo() {
            Console.WriteLine("Files Blaster");
            Console.WriteLine("Usage: Blaster <command>");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("no default values are configured except parallelism and ignore white space.");
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Where <command> is of: " +
                                "\n\t -a=fuse -s=sourceDirectory -t=targetDirectory -i=ignore " +
                                "\n\t -a=extract -p=async|sync  -s=sourceDirectory -t=targetDirectory -i=ignore");

        }

        static Dictionary<string, string> argsHandler(string[] args) {

            var arguments = new Dictionary<string, string>();

            foreach (string argument in args)
            {
                string[] splitted = argument.Split('=');

                if (splitted.Length == 2)
                {
                    arguments[splitted[0]] = splitted[1];
                }
            }

            return arguments;
        }
    }
}
