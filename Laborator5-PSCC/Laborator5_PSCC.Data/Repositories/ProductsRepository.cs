using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laborator5_PSSC.Domain.Repositories;
using Laborator5_PSSC.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Laborator5_PSCC.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ProductsContext productsContext;

        public ProductsRepository(ProductsContext productsContext)
        {
            this.productsContext = productsContext;
        }

        public TryAsync<List<ProductCode>> TryGetExistingProductCode(IEnumerable<string> productsToCheck) => async () =>
        {
            var products = await productsContext.Products
                                              .Where(product => productsToCheck.Contains(product.Code))
                                              .AsNoTracking()
                                              .ToListAsync();
            return products.Select(product => new ProductCode(product.Code))
                           .ToList();
        };

        public TryAsync<List<ProductQuantity>> TryGetStockProduct(IEnumerable<int> productsToCheck) => async () =>
        {
            var products = await productsContext.Products
                                              .Where(product => productsToCheck.Contains(product.Stoc))
                                              .AsNoTracking()
                                              .ToListAsync();
            return products.Select(product => new ProductQuantity(product.Stoc))
                           .ToList();
        };

    }
}
