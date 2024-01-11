using System.Collections.Generic;
using Exemple.Domain.Models;

namespace Exemple.Domain.Commands
{
    public record ProcessOrderCommand
    {

        public ProcessOrderCommand(IReadOnlyCollection<UnvalidatedProduct> inputShoppingCart)
        {
            InputShoppingCart = inputShoppingCart;
        }

        public IReadOnlyCollection<UnvalidatedProduct> InputShoppingCart { get; }
        public Client InputClientDetails { get; }
    }
}
