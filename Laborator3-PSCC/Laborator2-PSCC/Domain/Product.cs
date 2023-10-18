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
        private static readonly Regex ValidPatternCode = new("^[0-9]{6}$");
        public string? Code { get; set; }
        public decimal Price { get; set; }
        public Product(string code, decimal price)
        {
            if (IsValidCode(code))
            {
                Code = code;
            }
            if(IsValidPrice(price))
            {
                Price = price;
            }
        }
        public override string ToString()
        {
            return "Code=" + Code + " " + "Price=" + Price;
        }

        private static bool IsValidCode(string stringValue) => ValidPatternCode.IsMatch(stringValue);
        private static bool IsValidPrice(decimal numericPrice) => numericPrice > 0;
        public static bool TryParsePrice(string priceString, out decimal price)
        {
            bool isValid = false;
            price = 0;
            if (decimal.TryParse(priceString, out decimal numericPrice))
            {
                if (IsValidPrice(numericPrice))
                {
                    isValid = true;
                    price = numericPrice;
                }
            }
            return isValid;
        }
    }
}
