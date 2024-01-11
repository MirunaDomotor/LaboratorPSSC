using Exemple.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record Client
    {
        private static readonly Regex ValidPatternName = new("^[a-zA-Z]{1,20}$");
        private static readonly Regex ValidPatternAddress = new("^[a-zA-Z][a-zA-Z0-9]{1,20}$");
        public string Name { get; set; }
        public string Address { get; set; }

        public Client(string name, string address)
        {
            if (IsNameValid(name) && IsAddressValid(address))
            {
                this.Name = name;
                this.Address = address;
            }
            else
            {
                throw new InvalidClientException("The format of the client name is wrong! It should only contain letters!");
            }
            if(IsAddressValid(address))
            {
                this.Address = address;
            }
            else 
            { 
                throw new InvalidClientException("The format of the client address is wrong! It should only contain letters and digits!"); 
            }
        }

        private static bool IsNameValid(string? stringValue) => ValidPatternName.IsMatch(stringValue);
        private static bool IsAddressValid(string? stringValue) => ValidPatternAddress.IsMatch(stringValue);
    }
}
