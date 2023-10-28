using System;
using System.Text.RegularExpressions;

namespace Laborator3_PSCC.Domain.Models
{
	public record ProductCode
	{
        private static readonly Regex ValidPattern = new("^[0-9]{6}$");

        public string Value { get; set; }

		public ProductCode(string value)
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

        public static bool TryParse(string stringValue, out ProductCode? productCode )
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

