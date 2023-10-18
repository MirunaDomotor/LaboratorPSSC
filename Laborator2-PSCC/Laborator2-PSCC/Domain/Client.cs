using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record Client
    {
        public string? Name { get; set; }
        public string? Address { get; set; }

        public Client(string? name, string? address)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}
