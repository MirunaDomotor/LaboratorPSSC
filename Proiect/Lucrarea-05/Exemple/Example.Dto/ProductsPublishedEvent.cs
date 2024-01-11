
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Dto
{
    public record ProductsPublishedEvent
    {
        public List<ProductCartDto> Products { get; init; }
    }
}
