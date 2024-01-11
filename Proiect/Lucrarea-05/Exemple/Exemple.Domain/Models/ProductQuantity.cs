using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record ProductQuantity
    {
        public int Value { get; set; }

        public ProductQuantity(int value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidProductQuantityException("The format of product quantity is wrong! It should be a positive number!");
            }
        }

        public int ReturnQuantity()
        {
            return Value;
        }

        private static bool IsValid(int numericQuantity) => numericQuantity > 0;


        public static Option<ProductQuantity> TryParse(string valueString)
        {
            if (int.TryParse(valueString, out int numericQuantity) && IsValid(numericQuantity))
            {
                return Some<ProductQuantity>(new(numericQuantity));
            }
            else
            {
                return None;
            }
        }
    }
}
