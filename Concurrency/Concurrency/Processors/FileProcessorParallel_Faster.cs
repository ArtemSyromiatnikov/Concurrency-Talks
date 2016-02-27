using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency_Sequential.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Concurrent;

namespace Concurrency_Sequential.Processors
{
    /// <summary>
    /// Eliminate shared Regex variable to reduce contention.
    /// </summary>
    public class FileProcessorParallel_Faster : IFileProcessor
    {
        // Example string:     COMPLETE 590229dd-601a-4e5d-8356-477bb86a9fd7, Tele2, '11/09/2015 12:57:11'
        public static readonly string PatternString = @"COMPLETE ([\d\w-]{36}), (\w+), '(.*)'";
        public static readonly string Tele2Brand = @"Tele2";

        public ProcessingResult ProcessFiles(IEnumerable<string> files)
        {
            List<Task<ProcessingResult>> tasks = new List<Task<ProcessingResult>>();
            foreach (string fileName in files)
            {
                Task<ProcessingResult> task = Task.Factory.StartNew<ProcessingResult>((arg) =>
                {
                    string localFileName = (string)arg;
                    ProcessingResult localResult = new ProcessingResult();
                    
                    byte[] bytes = File.ReadAllBytes(localFileName);
                    string body = Encoding.UTF8.GetString(bytes);

                    Regex pattern = new Regex(PatternString, RegexOptions.Compiled | RegexOptions.Multiline);
                    Match match = pattern.Match(body);
                    while (match.Success)
                    {
                        localResult.Total++;

                        if (match.Groups[2].Value == Tele2Brand)
                        {
                            Guid id = Guid.Parse(match.Groups[1].Value);
                            localResult.Tele2OrderIds.Add(id);
                        }

                        match = match.NextMatch();
                    }

                    return localResult;
                },
                fileName);
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            // Aggregate the results
            ProcessingResult aggregatedResult = new ProcessingResult();
            foreach (var task in tasks)
            {
                aggregatedResult.Total += task.Result.Total;
                aggregatedResult.Tele2OrderIds.AddRange(task.Result.Tele2OrderIds);
            }

            return aggregatedResult;
        }
    }
}
