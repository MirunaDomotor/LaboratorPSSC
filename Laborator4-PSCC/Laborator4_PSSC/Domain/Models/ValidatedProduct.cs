using System;
using System.Diagnostics;
using Laborator4_PSCC.Domain.Models;
using Laborator3_PSSC.Domain.Models;

namespace Laborator4_PSCC.Domain
{
    public record ValidatedProduct(ProductCode Code, ProductQuantity Quantity, ProductPrice Price);
}

