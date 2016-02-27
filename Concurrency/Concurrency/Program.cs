using Concurrency_Sequential.Data;
using Concurrency_Sequential.Processors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency_Sequential
{
    class Program
    {
        public static readonly string DataSet0Folder = @"D:\Projects\dotNET\Concurrency\TEST FILES\Set 0 - simple";
        public static readonly string DataSet1Folder = @"D:\Projects\dotNET\Concurrency\TEST FILES\Set 1 - random hits";
        public static readonly string DataSet2Folder = @"D:\Projects\dotNET\Concurrency\TEST FILES\Set 2 - many hits";

        //public static IFileProcessor FileProcessor = new FileProcessorSequential();
        //public static IFileProcessor FileProcessor = new FileProcessorParallel_Sync();
        //public static IFileProcessor FileProcessor = new FileProcessorParallel_Sync_V2();
        //public static IFileProcessor FileProcessor = new FileProcessorParallel_Queue();
        //public static IFileProcessor FileProcessor = new FileProcessorParallel_LockFree();
        public static IFileProcessor FileProcessor = new FileProcessorParallel_Faster();

        static void Main(string[] args)
        {
            Console.WriteLine("Concurrency Demo");
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("Select which data set to process:");
                Console.WriteLine("  0. Single file");
                Console.WriteLine("  1. 50 files with random hits");
                Console.WriteLine("  2. 50 files where every line hits");
                Console.Write("Your choice: ");
                string input = Console.ReadLine();

                switch(input)
                {
                    case "0":
                        ProcessFiles(DataSet0Folder);
                        break;
                    case "1":
                        ProcessFiles(DataSet1Folder);
                        break;
                    case "2":
                        ProcessFiles(DataSet2Folder);
                        break;
                    default:
                        isRunning = false;
                        break;
                }
            }
        }

        private static void ProcessFiles(string folderPath)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(folderPath);

            Console.WriteLine("Processing {0} files...", files.Count());
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ProcessingResult result = FileProcessor.ProcessFiles(files);
            stopwatch.Stop();

            FormattedOutput(stopwatch, result);
        }
        
        private static void FormattedOutput(Stopwatch stopwatch, ProcessingResult result)
        {
            var defaultColor = Console.ForegroundColor;
        
            Console.WriteLine();

            Console.Write("Total number of COMPLETED entries: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(result.Total);
            Console.ForegroundColor = defaultColor;

            Console.Write("COMPLETED entries in Tele2 domain: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(result.Tele2OrderIds.Count);
            Console.ForegroundColor = defaultColor;

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("time elapsed: {0}ms", stopwatch.ElapsedMilliseconds);
            Console.ForegroundColor = defaultColor;

            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
