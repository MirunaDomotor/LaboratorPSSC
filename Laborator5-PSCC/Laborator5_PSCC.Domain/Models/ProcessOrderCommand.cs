using System;
using Laborator5_PSSC.Domain;

namespace Laborator5_PSSC.Domain
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