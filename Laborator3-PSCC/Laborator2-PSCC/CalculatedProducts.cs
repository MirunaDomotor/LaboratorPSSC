using Laborator2_PSCC.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC
{
    public record CalculatedProducts(ValidatedProducts cart, string price)
    {
    }
}
