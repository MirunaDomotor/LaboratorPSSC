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
    }
}
