using LanguageExt;
using System;
using System.Text.RegularExpressions;
using static LanguageExt.Prelude;

namespace Exemple.Domain.Models
{
    public record Product
    {
        private static readonly Random random = new Random();
        private static readonly Regex ValidPatternCode = new("^[0-9]{6}$");
        public ProductCode Code { get; set; }

        public ProductQuantity Quantity { get; set; }

        public ProductStock Stock { get; set; }
        public ProductPrice Price { get; set; }

        public int ProductId { get; set; }
        public Product(ProductCode code, ProductQuantity quantity, ProductStock stock, ProductPrice price)
        {
            Code = code;
            Quantity = quantity;
            Stock = stock;
            Price = price;
        }

        private static bool IsValidCode(string stringValue) => ValidPatternCode.IsMatch(stringValue);
        public static bool TryParseCode(string stringValue, out string? code)
        {
            bool isValid = false;
            code = null;
            if (IsValidCode(stringValue))
            {
                isValid = true;
                code = new(stringValue);
            }
            return isValid;
        }

        private static bool IsValidQuantity(int numericQuantity) => numericQuantity > 0;
        public static bool TryParseQuantity(string quantityString, out int quantity)
        {
            bool isValid = false;
            quantity = 0;
            if (int.TryParse(quantityString, out int numericQuantity))
            {
                if (IsValidQuantity(numericQuantity))
                {
                    isValid = true;
                    quantity = numericQuantity;
                }
            }
            return isValid;
        }


    }
}
