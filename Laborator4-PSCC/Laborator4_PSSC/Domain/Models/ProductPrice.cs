using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator3_PSSC.Domain.Models
{
   public record ProductPrice
    {
        private static readonly Random random = new Random();
        public double Value;
        public ProductPrice()
        {
            Value = System.Math.Round(random.Next(200) * random.NextDouble(), 2);
        }
        public double ReturnPrice()
        {
            return Value;
        }
    }   
}
