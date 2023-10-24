using System;
using System.Text.RegularExpressions;

namespace Laborator3_PSCC.Domain.Models
{
	public class ProductCodeValidation
	{
        private static readonly Regex ValidPattern = new("^[0-9]{6}$");

        public string Value { get; set; }

		public ProductCodeValidation(string value)
		{
			if (IsValid(value))
			{
				Value = value;
			}
			else
			{
				throw new InvalidProductCodeException("");
			}
		}

        private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);

        public override string ToString()
        {
            return Value;
        }

        public static bool TryParse(string stringValue, out ProductCodeValidation? productCode )
        {
            bool isValid = false;
            productCode = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                productCode = new(stringValue);
            }

            return isValid;
        }
    }
}

