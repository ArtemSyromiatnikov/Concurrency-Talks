using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency_Sequential.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Concurrency_Sequential.Processors
{
    public class FileProcessorSequential : IFileProcessor
    {
        // Example string:     COMPLETE 590229dd-601a-4e5d-8356-477bb86a9fd7, Tele2, '11/09/2015 12:57:11'
        public static readonly string PatternString = @"COMPLETE ([\d\w-]{36}), (\w+), '(.*)'";
        public static readonly string Tele2Brand = @"Tele2";

        public ProcessingResult ProcessFiles(IEnumerable<string> files)
        {
            Regex pattern = new Regex(PatternString, RegexOptions.Compiled | RegexOptions.Multiline);

            int count = 0;
            List<Guid> tele2OrderIds = new List<Guid>();


            foreach (string fileName in files)
            {
                byte[] bytes = File.ReadAllBytes(fileName);
                string body = Encoding.UTF8.GetString(bytes);

                Match match = pattern.Match(body);

                while (match.Success)
                {
                    count++;

                    if (match.Groups[2].Value == Tele2Brand)
                    {
                        Guid id = Guid.Parse(match.Groups[1].Value);
                        tele2OrderIds.Add(id);
                    }

                    match = match.NextMatch();
                }
            }


            return new ProcessingResult
            {
                Total = count,
                Tele2OrderIds = tele2OrderIds
            };
        }
    }
}
