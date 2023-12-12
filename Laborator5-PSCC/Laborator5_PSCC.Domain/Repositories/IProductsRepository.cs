using Laborator5_PSSC.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator5_PSSC.Domain.Repositories
{
    public interface IProductsRepository
    {
        TryAsync<List<ProductCode>> TryGetExistingProductCode(IEnumerable<string> productsToCheck);
        TryAsync<List<ProductQuantity>> TryGetStockProduct(IEnumerable<int> productsToCheck);
    }
}
