using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Commands
{
    public record OrderModificationCommand
    {

        public OrderModificationCommand(IReadOnlyCollection<UnvalidatedProduct> inputShoppingCart)
        {
            InputShoppingCart = inputShoppingCart;
        }

        public IReadOnlyCollection<UnvalidatedProduct> InputShoppingCart { get; }
        public Client InputClientDetails { get; }
    }
}
