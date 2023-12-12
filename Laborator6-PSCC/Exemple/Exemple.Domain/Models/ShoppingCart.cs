using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public record ShoppingCart
    {
        public Client? Client { get; set; }
        public List<Product>? Products { get; set; }

    }
}
