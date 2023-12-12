using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator5_PSSC.Domain.ShoppingCartChoice;

namespace Laborator5_PSCC.Domain.Repositories
{
    public interface IOrdersRepository
    {
        public TryAsync<Unit> TrySaveOrder(PaidShoppingCart cart);
    }
}
