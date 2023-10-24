using System;
using System.Diagnostics;
using Laborator3_PSCC.Domain.Models;

namespace Laborator3_PSCC.Domain
{
    public record ValidatedProduct(ProductCodeValidation Code, ProductQuantityValidation Quantity, double Price);
}

