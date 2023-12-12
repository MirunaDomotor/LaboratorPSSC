using Laborator5_PSSC.Domain.Models;

namespace Laborator5_PSSC.Domain
{
    public record ValidatedProduct(ProductCode Code, ProductQuantity Quantity, ProductPrice Price);
}

