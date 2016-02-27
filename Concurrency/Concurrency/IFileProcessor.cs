using System.Collections.Generic;
using Concurrency_Sequential.Data;

namespace Concurrency_Sequential
{
    public interface IFileProcessor
    {
        ProcessingResult ProcessFiles(IEnumerable<string> files);
    }
}