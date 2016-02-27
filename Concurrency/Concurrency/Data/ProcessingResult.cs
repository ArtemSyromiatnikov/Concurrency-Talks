using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency_Sequential.Data
{
    public class ProcessingResult
    {
        public int Total { get; set; }
        public List<Guid> Tele2OrderIds {get;set;}

        public ProcessingResult()
        {
            Tele2OrderIds = new List<Guid>();
        }
    }
}
