using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record ProductPrice
    {
        private static readonly Random random = new Random();
        public double Value;
        public ProductPrice(double value)
        {
            Value = value; 
            //Value = Math.Round(random.Next(200) * random.NextDouble(), 2);
        }
        public double ReturnPrice()
        {
            return Value;
        }

        public static Option<ProductPrice> TryParse(string valueString)
        {
            if (double.TryParse(valueString, out double numericPrice))
            {
                return Some<ProductPrice>(new(numericPrice));
            }
            else
            {
                return None;
            }
        }
    }
}
