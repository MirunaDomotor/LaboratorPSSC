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
        public record EmptyCart : IShoppingCart
        {
            public EmptyCart() { }
        }
        public record UnvalidatedCart : IShoppingCart
        {
            public IReadOnlyCollection<UnvalidatedProducts> ProductsList { get; }
            public UnvalidatedCart(IReadOnlyCollection<UnvalidatedProducts> productsList) 
            {
                ProductsList = productsList;
            }
        }
        public record InvalidatedCart : IShoppingCart
        {
            public IReadOnlyCollection<UnvalidatedProducts> ProductsList { get; }
            public string Reason { get; }
            internal InvalidatedCart(IReadOnlyCollection<UnvalidatedProducts> productsList, string reasonForInvalidation)
            {
                ProductsList = productsList;
                Reason = reasonForInvalidation;
            }
        }
        public record ValidatedCart : IShoppingCart
        {
            public IReadOnlyCollection<ValidatedProducts> ProductsList { get; }
            internal ValidatedCart(IReadOnlyCollection<ValidatedProducts> productsList)
            {
                ProductsList = productsList;
            }
        }

        public record CalculatedCart : IShoppingCart
        {
            internal CalculatedCart(IReadOnlyCollection<CalculatedCart> productsList)
            {
                ProductsList = productsList;
            }

            public IReadOnlyCollection<CalculatedCart> ProductsList { get; }
        }

        public record PaidCart : IShoppingCart
        {
            public IReadOnlyCollection<ValidatedProducts> ProductsList { get; }
            public PaymentInfo PaymentInfo { get; }
            public PaidCart(IReadOnlyCollection<ValidatedProducts> productsList, PaymentInfo paymentInfo)
            {
                ProductsList = productsList;
                PaymentInfo = paymentInfo;
            }
        }
    }
}
