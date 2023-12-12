using Exemple.Domain.Models;
using LanguageExt;
using System.Collections.Generic;

namespace Exemple.Domain.Repositories
{
    public interface IProductsRepository
    {
        TryAsync<List<ProductCode>> TryGetExistingProductCode(IEnumerable<string> productsToCheck);
        TryAsync<List<ProductQuantity>> TryGetStockProduct(IEnumerable<int> productsToCheck);
    }
}
