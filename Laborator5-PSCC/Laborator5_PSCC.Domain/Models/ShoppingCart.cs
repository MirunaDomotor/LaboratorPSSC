using System;
namespace Laborator5_PSSC.Domain
{
	public record ShoppingCart
	{ 
		public Client? Client { get; set; }
		public List<Product>? Products { get; set; }
		
	}
}

