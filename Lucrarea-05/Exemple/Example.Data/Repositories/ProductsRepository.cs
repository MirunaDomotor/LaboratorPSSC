using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Example.Data.Repositories
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
            return products.Select(product => new ProductQuantity(product.Stoc, product.Stoc))
                           .ToList();
        };

    }
}
