using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static Laborator1PSCC.Quantity;

namespace Laborator1PSCC
{
    internal record Product
    {
        public string Code { get; set; }
        public IQuantity Quantity { get; }
        public Product(string code, IQuantity quantity)
        {
            Code = code;
            Quantity = quantity;
        }
        public void Write()
        {
            Console.WriteLine("Code=" + Code + " " + "Quantity" + ":" + Quantity + "\n");
        }
    }
}