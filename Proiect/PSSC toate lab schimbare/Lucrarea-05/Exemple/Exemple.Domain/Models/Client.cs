using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record Client
    {
        //private static readonly Regex ValidPatternName = new("^[a-zA-Z]{5,20}$");
        //private static readonly Regex ValidPatternAddress = new("^[a-zA-Z][a-zA-Z0-9]*$");
        public string Name { get; set; }
        public string Address { get; set; }

        public Client(string Name, string Address)
        {
            this.Name = Name;
            this.Address = Address;
        }

        //private static bool IsNameValid(string? stringValue) => ValidPatternName.IsMatch(stringValue);
        //private static bool IsAddressValid(string? stringValue) => ValidPatternAddress.IsMatch(stringValue);
    }
}
