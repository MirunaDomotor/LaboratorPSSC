using System;
using System.Diagnostics;

namespace Laborator3_PSCC.Domain.Models
{
	public record CalculatedPrice(ProductCodeValidation Code, ProductQuantityValidation Quantity, double TotalPrice);
}