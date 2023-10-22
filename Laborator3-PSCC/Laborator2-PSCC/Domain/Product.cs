using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record Product
    {
        private static readonly Random random = new Random();
        private static readonly Regex ValidPatternCode = new("^[0-9]{6}$");
        public string? Code { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public Product(string code, int quantity)
        {
            if (IsValidCode(code))
            {
                Code = code;
            }
            if(IsValidQuantity(quantity))
            {
                Quantity = quantity;
                Price = random.Next(100) * Quantity;
            }
        }
        public override string ToString()
        {
            return "Code=" + Code + " " + "Quantity=" + Quantity +  "Price=" + Price;
        }

        private static bool IsValidCode(string stringValue) => ValidPatternCode.IsMatch(stringValue);
        public static bool TryParseCode(string stringValue, out string? code)
        {
            bool isValid = false;
            code = null;
            if(IsValidCode(stringValue))
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
