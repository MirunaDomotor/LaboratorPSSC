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
        public double Value;
        public ProductPrice(double value)
        {
            Value = value; 
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
