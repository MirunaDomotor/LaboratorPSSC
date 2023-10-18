using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    [AsChoice]
    public static partial class ShoppingCart
    {
        public interface IShoppingCart { }
        public record EmptyCart() : IShoppingCart;
        public record UnvalidatedCart(IReadOnlyCollection<UnvalidatedProducts> ProductsList) : IShoppingCart;
        public record InvalidatedCart(IReadOnlyCollection<UnvalidatedProducts> ProductsList, string reasonForInvalidation) : IShoppingCart;
        public record ValidatedCart(IReadOnlyCollection<ValidatedProducts> ProductsList) : IShoppingCart;
        public record PaidCart(IReadOnlyCollection<ValidatedProducts> ProductsList, PaymentInfo paymentInfo) : IShoppingCart;
    }
}
