using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record Product
    {
        public string? Code { get; set; }
        public Product(string code)
        {
            Code = code;
        }
        public override string ToString()
        {
            return Code;
        }
    }
}
