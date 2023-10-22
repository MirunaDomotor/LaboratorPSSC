using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator2_PSCC.Domain.ShoppingCart;

namespace Laborator2_PSCC.Domain
{
    public record Account
    {
        //public List<Product>? Products { get; set; }
        public Client? Client { get; set; }
        public IShoppingCart Cart { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public Account(Client client, IShoppingCart cart, PaymentInfo paymentInfo)
        {
            Client = client;
            Cart = cart;
            PaymentInfo = paymentInfo;
        }

    }
}
