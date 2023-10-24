using System;
namespace Laborator3_PSCC.Domain
{
	public record ShoppingCart
	{ 
		public Client? Client { get; set; }
		public List<Product>? Products { get; set; }
		
	}
}

