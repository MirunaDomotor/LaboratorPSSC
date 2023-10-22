using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record UnvalidatedProducts(string Code, string Quantity, string Price)
    {
    }
}
