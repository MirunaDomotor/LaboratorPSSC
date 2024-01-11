using Exemple.Domain.Models;
using LanguageExt;
using System.Collections.Generic;

namespace Exemple.Domain.Repositories
{
    public interface IProductsRepository
    {
        public TryAsync<ProductCode> TryGetExistingProductCode(IEnumerable<string> productsToCheck);
        public TryAsync<List<Product>> TryGetExistingProductsDeposit();
        TryAsync<ProductStock> TryGetStockProduct(string productCode);
    }
}
