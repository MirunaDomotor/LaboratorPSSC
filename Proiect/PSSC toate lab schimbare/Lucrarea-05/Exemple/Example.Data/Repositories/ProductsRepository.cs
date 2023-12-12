using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static LanguageExt.Prelude;

namespace Example.Data.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ProductsContext productsContext;

        public ProductsRepository(ProductsContext productsContext)
        {
            this.productsContext = productsContext;
        }

        //public TryAsync<List<ProductCode>> TryGetExistingProductCode(IEnumerable<string> productsToCheck) => async () =>
        //{
        //    var products = await productsContext.Products
        //                                      .Where(product => productsToCheck.Contains(product.Code))
        //                                      .AsNoTracking()
        //                                      .ToListAsync();
        //    return products.Select(product => new ProductCode(product.Code))
        //                   .ToList();
        //};

        public TryAsync<ProductCode> TryGetExistingProductCode(IEnumerable<string> productsToCheck) => async () =>
        {
            var product = await productsContext.Products.FirstOrDefaultAsync(product => productsToCheck.Contains(product.Code));

            if (product != null)
            {
                return new ProductCode(product.Code);
            }
            else
            {
                return new ProductCode("000000");
            }
        };

        //public TryAsync<List<ProductStock>> TryGetStockProduct(IEnumerable<int> productsToCheck) => async () =>
        //{
        //    var products = await productsContext.Products
        //                                      .Where(product => productsToCheck.Contains(product.Stoc))
        //                                      .AsNoTracking()
        //                                      .ToListAsync();
        //    return products.Select(product => new ProductStock(product.Stoc))
        //                   .ToList();
        //};

        public TryAsync<ProductStock> TryGetStockProduct(string productCode) => async () =>
        {
            var product = await productsContext.Products.FirstOrDefaultAsync(p => p.Code == productCode);

            if (product != null)
            {
                return new ProductStock(product.Stoc);
            }
            else
            {
                return new ProductStock(100);
            }
        };

        public TryAsync<List<Product>> TryGetExistingProductsDeposit() => async () => (await (
                  from s in productsContext.Products
                  select new { s.Code, s.Stoc, s.Price })
                  .AsNoTracking()
                  .ToListAsync())
                  .Select(result => new Product(
                                            code: new ProductCode(result.Code),
                                            quantity: new ProductQuantity(result.Stoc),
                                            stock: new ProductStock(result.Stoc),
                                            price: new ProductPrice((double)result.Price)))
                  .ToList();

    }
}
