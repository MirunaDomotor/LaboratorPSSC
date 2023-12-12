using Exemple.Domain.Models;
using LanguageExt;
using System.Collections.Generic;

namespace Exemple.Domain.Repositories
{
    public interface IProductsRepository
    {
        //TryAsync<List<ProductCode>> TryGetExistingProductCode(IEnumerable<string> productsToCheck);
        public TryAsync<ProductCode> TryGetExistingProductCode(IEnumerable<string> productsToCheck);
        public TryAsync<List<Product>> TryGetExistingProductsDeposit();
        TryAsync<ProductStock> TryGetStockProduct(string productCode);
    }
}
