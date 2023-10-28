using System;
using Laborator4_PSCC.Domain;

namespace Laborator4_PSCC.Domain
{
	public record ProcessOrderCommand
	{
		
		public ProcessOrderCommand(IReadOnlyCollection<UnvalidatedProduct> inputShoppingCart)
		{
			InputShoppingCart = inputShoppingCart;
		}

		public IReadOnlyCollection<UnvalidatedProduct> InputShoppingCart { get;  }
	}
}