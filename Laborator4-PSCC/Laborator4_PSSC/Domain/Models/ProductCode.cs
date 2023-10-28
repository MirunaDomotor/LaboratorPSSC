using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Laborator4_PSCC.Domain.Models
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

        public static Option<ProductCode> TryParse(string stringValue)
        {
            if(IsValid(stringValue))
            {
                return Some<ProductCode>(new(stringValue));
            }
            else 
            {
                return None;
            }
        }
    }
}

