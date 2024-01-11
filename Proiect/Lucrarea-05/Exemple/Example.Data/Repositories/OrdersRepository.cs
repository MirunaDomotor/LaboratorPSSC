using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using LanguageExt;
using Example.Data.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static Exemple.Domain.Models.ShoppingCartChoice;
using static LanguageExt.Prelude;
using System;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Microsoft.Extensions.Options;

namespace Example.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly ProductsContext dbContext;

        public OrdersRepository(ProductsContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public TryAsync<List<CalculatedPrice>> TryGetExistingProducts() => async () => (await (
                  from g in dbContext.OrderLines
                  join s in dbContext.Products on g.ProductId equals s.ProductId
                  select new { s.Code, s.Stoc, s.Price, g.OrderLineId, g.Quantity})
                  .AsNoTracking()
                  .ToListAsync())
                  .Select(result => new CalculatedPrice(
                                            Code: new ProductCode(result.Code),
                                            Quantity: new ProductQuantity((int)result.Quantity),
                                            Stock: new ProductStock(result.Stoc),
                                            Price: new ProductPrice((double)result.Price),
                                            TotalPrice: (double)(result.Quantity * result.Price))
                  {
                      OrderLineId = result.OrderLineId
                  })
                  .ToList();

        public TryAsync<Unit> TrySaveOrder(PaidShoppingCart cart) => async () =>
        {
            var products = (await dbContext.Products.ToListAsync()).ToLookup(product => product.Code);
            var newCart = cart.ProductsList
                                    .Where(g => g.IsUpdated && g.OrderLineId == 0)
                                    .Select(g => new OrderLineDto()
                                    {
                                        ProductId = products[g.Code.Value].Single().ProductId,
                                        Quantity = g.Quantity.Value,
                                        //Price = g.TotalPrice,
                                        Price = products[g.Code.Value].Single().Price * g.Quantity.Value,
                                    });
            var updatedCart = cart.ProductsList.Where(g => g.IsUpdated && g.OrderLineId > 0)
                                    .Select(g => new OrderLineDto()
                                    {
                                        OrderLineId = g.OrderLineId,
                                        ProductId = products[g.Code.Value].Single().ProductId,
                                        Quantity = g.Quantity.Value,
                                        //Price = g.TotalPrice,
                                        Price = products[g.Code.Value].Single().Price * g.Quantity.Value,
                                    });

            dbContext.AddRange(newCart);
            foreach (var entity in updatedCart)
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            await dbContext.SaveChangesAsync();

            return unit;
        };


        public double CalculateFinalPrice()
        {
            double finalPrice = dbContext.OrderLines
         //.Select(g => (double)(g.Quantity * g.Price)) //era cand nu calculam pretul total al unui produs in OrderLine
         .Select(g=>(double)g.Price)
         .Sum();

            return finalPrice;
        }

        public void DeleteAllProductsFromShoppingCart()
        {
            dbContext.OrderLines.RemoveRange(dbContext.OrderLines);
            dbContext.SaveChanges();
        }

        public TryAsync<OrderInfo> TryGetClientOrderInfo(int clientId) => async () =>
        {
            var clientInfo = await dbContext.OrderHeaders
                .Where(c => c.OrderId == clientId)
                .Select(c => new { c.Name, c.Address, c.Total })
                .FirstOrDefaultAsync();

            if (clientInfo == null)
                return new OrderInfo();

            var orderInfo = await (
                from order in dbContext.OrderHeaders
                where order.OrderId == clientId
                select new OrderInfo(new Client(order.Name, order.Address), (double)order.Total))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return orderInfo;
        };

        public TryAsync<bool> TryOrderExists(int orderId) => async () =>
        {
            bool orderExists = await dbContext.OrderHeaders.AnyAsync(order => order.OrderId == orderId);

            if (orderExists)
                return true;
            else
                return false;
        };

        public TryAsync<Unit> TryCancelOrder(int orderId) => async () =>
        {
            var orderToRemove = await dbContext.OrderHeaders.FindAsync(orderId);

            if (orderToRemove != null)
            {
                dbContext.OrderHeaders.Remove(orderToRemove);
                int cancelOrderYesOrNo = 1;
                UpdateProductStock(cancelOrderYesOrNo);
                dbContext.OrderLines.RemoveRange(dbContext.OrderLines);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                // Tratați cazul în care OrderLine nu a fost găsit
                // Puteți arunca o excepție sau gestiona altfel situația
            }
            return unit;
        };

        public void UpdateProductStock(int cancelOrderYesOrNo)
        {
            try
            {
                var orderLines = dbContext.OrderLines.ToList();

                foreach (var orderLine in orderLines)
                {
                    var orderLineProductId = orderLine.ProductId;

                    var product = dbContext.Products
                        .Where(p => p.ProductId == orderLineProductId)
                        .FirstOrDefault();

                    if (product != null)
                    {
                        if (cancelOrderYesOrNo == 0)
                        {// Actualizează stocul produsului
                            product.Stoc -= orderLine.Quantity;
                        }
                        else
                        {
                            product.Stoc += orderLine.Quantity;
                        }

                        // Marchează entitatea ca modificată în contextul de date
                        dbContext.Entry(product).State = EntityState.Modified;
                    }
                }

                // Alte operațiuni aici...

                //dbContext.OrderLines.RemoveRange(dbContext.OrderLines);
                //DeleteAllProductsFromShoppingCart();

                //dbContext.OrderLines.RemoveRange(dbContext.OrderLines);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching product: {ex.Message}");
            }
        }

    }
}
