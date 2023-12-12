using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Dto
{
    public record ProductCartDto
    {
        public string Name { get; init; }
        public string Code { get; init; }
        public int Quantity { get; init; }
        public double Price { get; init; }
        public double TotalPrice { get; init; }
    }
}
