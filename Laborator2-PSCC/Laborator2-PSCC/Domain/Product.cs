using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record Product
    {
        private static readonly Random random = new Random();
        public string? Code { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Product(string code, int quantity)
        {
            Code = code;
            Quantity = quantity;
            Price = random.Next(100) * Quantity;
        }
        public override string ToString()
        {
            return "Code=" + Code + " " + "Quantity=" + Quantity + " " + "Price=" + Price;
        }
    }
}
