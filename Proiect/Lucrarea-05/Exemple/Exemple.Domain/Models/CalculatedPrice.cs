using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record CalculatedPrice(ProductCode Code, ProductQuantity Quantity, ProductStock Stock, ProductPrice Price, double TotalPrice)
    {
        public int OrderLineId { get; set; }
        public bool IsUpdated { get; set; }
    }
}
