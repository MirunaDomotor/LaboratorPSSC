using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record ProductStock
    {
        public int Stock { get; set; }

        public ProductStock(int stock)
        {
            if (IsValid(stock))
            {
                Stock = stock;
            }
            else
            {
                throw new InvalidProductQuantityException("");
            }
        }

        public int ReturnStock()
        {
            return Stock;
        }

        private static bool IsValid(int numericStock) => numericStock > 0;


        public static Option<ProductStock> TryParse(string valueString)
        {
            if (int.TryParse(valueString, out int numericStock) && IsValid(numericStock))
            {
                return Some<ProductStock>(new(numericStock));
            }
            else
            {
                return None;
            }
        }
    }
}
