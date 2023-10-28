using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Laborator3_PSCC.Domain.Models
{
    public record ProductQuantity
    {
        private static readonly Random random = new Random();
        public static int Stock = 12;

        public int Value { get; set; }

        public ProductQuantity(int value)
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

        public int ReturnQuantity()
        {
            return Value;
        }

        private static bool IsValid(int numericQuantity) => numericQuantity < Stock && numericQuantity > 0;


        public static bool TryParse(string valueString, out ProductQuantity? productQuantity)
        {
            bool isValid = false;
            productQuantity = null;
            if(int.TryParse(valueString, out int numericQuantity))
            {
                if (IsValid(numericQuantity))
                {
                    isValid = true;
                    productQuantity = new(numericQuantity);
                }
            }

            return isValid;
        }
    }
}

