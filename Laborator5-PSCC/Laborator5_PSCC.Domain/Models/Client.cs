using System;
using System.Text.RegularExpressions;

namespace Laborator5_PSSC.Domain
{
	public record Client
	{
        //private static readonly Regex ValidPatternName = new("^[a-zA-Z]{5,20}$");
        //private static readonly Regex ValidPatternAddress = new("^[a-zA-Z][a-zA-Z0-9]*$");
        public string? Name { get; set; }
        public string? Address { get; set; }

        public Client(string? name, string? address)
        {
            Name = name;
            Address = address;
        }

        //private static bool IsNameValid(string? stringValue) => ValidPatternName.IsMatch(stringValue);
        //private static bool IsAddressValid(string? stringValue) => ValidPatternAddress.IsMatch(stringValue);
    }
}

