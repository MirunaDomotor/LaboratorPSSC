using System;
namespace Laborator4_PSCC.Domain
{
	public record ShoppingCart
	{ 
		public Client? Client { get; set; }
		public List<Product>? Products { get; set; }
		
	}
}

