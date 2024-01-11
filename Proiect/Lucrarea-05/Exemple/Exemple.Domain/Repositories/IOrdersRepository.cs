using Exemple.Domain.Models;
using LanguageExt;
using System.Collections.Generic;
using static Exemple.Domain.Models.ShoppingCartChoice;

namespace Exemple.Domain.Repositories
{
    public interface IOrdersRepository
    {
        TryAsync<List<CalculatedPrice>> TryGetExistingProducts();
        public TryAsync<Unit> TrySaveOrder(PaidShoppingCart cart);
        public TryAsync<OrderInfo> TryGetClientOrderInfo(int clientId);
        public double CalculateFinalPrice();
        public void DeleteAllProductsFromShoppingCart();
        public void UpdateProductStock(int cancelOrderYesOrNo);
        public TryAsync<bool> TryOrderExists(int orderId);
        public TryAsync<Unit> TryCancelOrder(int orderId);
    }
}
