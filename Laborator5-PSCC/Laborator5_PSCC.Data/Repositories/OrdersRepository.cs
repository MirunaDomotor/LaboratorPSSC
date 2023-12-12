using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Laborator5_PSCC.Data.Models;
using Laborator5_PSCC.Domain.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Laborator5_PSSC.Domain.ShoppingCartChoice;
using static LanguageExt.Prelude;

namespace Laborator5_PSCC.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly ProductsContext dbContext;

        public OrdersRepository(ProductsContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public TryAsync<Unit> TrySaveOrder(PaidShoppingCart cart) => async () =>
        {
            var products = (await dbContext.Products.ToListAsync()).ToLookup(product => product.Code);
            var newCart = cart.ProductsList
                                    .Where(g => g.IsUpdated && g.OrderLineId == 0)
                                    .Select(g => new OrderLineDto()
                                    {
                                        ProductId = products[g.Code.Value].Single().ProductId,
                                        Quantity = g.Quantity.Value,
                                        Price = g.Price.Value,
                                    });
            var updatedCart = cart.ProductsList.Where(g => g.IsUpdated && g.OrderLineId > 0)
                                    .Select(g => new OrderLineDto()
                                    {
                                        OrderLineId = g.OrderLineId,
                                        ProductId = products[g.Code.Value].Single().ProductId,
                                        Quantity = g.Quantity.Value,
                                        Price = g.Price.Value,
                                    });

            dbContext.AddRange(newCart);
            foreach (var entity in updatedCart)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return unit;
        };

    }
}
