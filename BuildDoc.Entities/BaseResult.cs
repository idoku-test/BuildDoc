using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildDoc.Entities
{
    public class BaseResult
    {
        public BaseResult()
        {
            Errors = new List<string>();
        }

        public bool Succeeded { get; set; }
        public decimal ResultId { get; set; }
        public List<string> Errors { get; set; }

        public string Error { get; set; }
    }
}
