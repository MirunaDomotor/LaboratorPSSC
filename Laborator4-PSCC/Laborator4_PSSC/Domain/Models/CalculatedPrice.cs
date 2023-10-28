using Laborator3_PSSC.Domain.Models;
using System;
using System.Diagnostics;

namespace Laborator4_PSCC.Domain.Models
{
	public record CalculatedPrice(ProductCode Code, ProductQuantity Quantity,ProductPrice Price, double TotalPrice);
}