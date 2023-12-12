using System;
using System.Diagnostics;

namespace Laborator5_PSSC.Domain.Models
{
	public record CalculatedPrice(ProductCode Code, ProductQuantity Quantity, ProductPrice Price, double TotalPrice)
	{
		public int OrderLineId { get; set; }
		public bool IsUpdated { get; set; }
	}
}