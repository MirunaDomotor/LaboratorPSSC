using System;
using Laborator3_PSCC.Domain;

namespace Laborator3_PSCC.Domain
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