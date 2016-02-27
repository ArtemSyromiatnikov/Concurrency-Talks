﻿using System;
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
    /// Using Thread-safe objects (Concurrent Queue)
    /// </summary>
    public class FileProcessorParallel_Queue : IFileProcessor
    {
        // Example string:     COMPLETE 590229dd-601a-4e5d-8356-477bb86a9fd7, Tele2, '11/09/2015 12:57:11'
        public static readonly string PatternString = @"COMPLETE ([\d\w-]{36}), (\w+), '(.*)'";
        public static readonly string Tele2Brand = @"Tele2";

        public ProcessingResult ProcessFiles(IEnumerable<string> files)
        {
            Regex pattern = new Regex(PatternString, RegexOptions.Compiled | RegexOptions.Multiline);

            int count = 0;
            ConcurrentQueue<Guid> tele2OrderIds = new ConcurrentQueue<Guid>();

            List<Task> tasks = new List<Task>();

            foreach (string fileName in files)
            {
                Task task = Task.Factory.StartNew((arg) =>
                {
                    string localFileName = (string)arg;
                    byte[] bytes = File.ReadAllBytes(localFileName);
                    string body = Encoding.UTF8.GetString(bytes);

                    Match match = pattern.Match(body);
                    while (match.Success)
                    {
                        Interlocked.Increment(ref count);

                        if (match.Groups[2].Value == Tele2Brand)
                        {
                            Guid id = Guid.Parse(match.Groups[1].Value);
                            tele2OrderIds.Enqueue(id);      // Add to concurrent queue
                        }

                        match = match.NextMatch();
                    }
                },
                fileName);
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());


            return new ProcessingResult
            {
                Total = count,
                Tele2OrderIds = tele2OrderIds.ToList()
            };
        }
    }
}
