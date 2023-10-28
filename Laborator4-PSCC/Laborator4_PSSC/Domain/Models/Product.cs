using Laborator4_PSCC.Domain.Models;
using Laborator3_PSSC.Domain.Models;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Laborator4_PSCC.Domain
{
	public record Product
	{
        private static readonly Random random = new Random();
        private static readonly Regex ValidPatternCode = new("^[0-9]{6}$");
        public ProductCode Code { get; set; }
        public ProductQuantity Quantity { get; set; }
        public ProductPrice Price { get; set; }
        public Product(ProductCode code, ProductQuantity quantity, ProductPrice price)
        {
            Code = code;
            Quantity = quantity;
            Price = price;
        }
        public override string ToString()
        {
            return "Code=" + Code + " " + "Quantity=" + Quantity +  "Price=" + Price;
        }

        public double CalculateTotalPrice()
        {
            double price = random.Next(200) * random.NextDouble();
            double totalPrice = System.Math.Round(Quantity.ReturnQuantity() * Price.ReturnPrice(), 2);
            return totalPrice;
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

