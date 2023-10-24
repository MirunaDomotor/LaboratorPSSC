using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Laborator3_PSCC.Domain.Models
{
    public record ProductQuantityValidation
    {
        private static readonly Random random = new Random();
        public static int Stock = 12;

        public int Value { get; set; }

        public ProductQuantityValidation(int value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidProductQuantityException("");
            }
        }

        public double CalculateTotalPrice()
        {
            double price = random.Next(200) * random.NextDouble();
            double totalPrice = System.Math.Round(Value * price, 2);
            return totalPrice;
        }

        private static bool IsValid(int stringValue) => stringValue < Stock;


        public static bool TryParse(int intValue, out ProductQuantityValidation? productQuantity)
        {
            bool isValid = false;
            productQuantity = null;

            if (IsValid(intValue))
            {
                isValid = true;
                productQuantity = new(intValue);
            }

            return isValid;
        }
    }
}

