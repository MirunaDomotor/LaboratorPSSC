using System.Collections.Generic;

namespace Exemple.Domain.Models
{
    public record ProcessOrderCommand
    {

        public ProcessOrderCommand(IReadOnlyCollection<UnvalidatedProduct> inputShoppingCart)
        {
            InputShoppingCart = inputShoppingCart;
        }

        public IReadOnlyCollection<UnvalidatedProduct> InputShoppingCart { get; }
    }
}
