using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator2_PSCC.Domain.ShoppingCart;

namespace Laborator2_PSCC.Domain
{
    public record Account
    {
        public Client? Client { get; set; }
        public IShoppingCart Cart { get; set; }
        public int? Quantity { get; set; }
        public PaymentInfo PaymentInfo { get; set; }
        public Account(Client client, IShoppingCart cart, int quantityOfProducts,PaymentInfo paymentInfo)
        {
            Client = client;
            Cart = cart;
            Quantity = quantityOfProducts;
            PaymentInfo = paymentInfo;
        }
        public static bool TryParseQuantity(string quantityString, out int quantity)
        {
            bool isValid = false;
            quantity = 0;
            if (int.TryParse(quantityString, out int numericQuantity))
            {
                if (IsValid(numericQuantity))
                {
                    isValid = true;
                    quantity = numericQuantity;
                }
            }

            return isValid;
        }
        private static bool IsValid(int numericQuantity) => numericQuantity > 0;

    }
}
