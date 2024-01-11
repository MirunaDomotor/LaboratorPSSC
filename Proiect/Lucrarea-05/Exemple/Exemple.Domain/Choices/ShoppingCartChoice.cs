using CSharp.Choices;
using System;
using System.Collections.Generic;

namespace Exemple.Domain.Models
{
    [AsChoice]
    public static partial class ShoppingCartChoice
    {
        public interface IShoppingCart { }
        public record EmptyShoppingCart : IShoppingCart
        {
            public ShoppingCart EmptyCart { get; }
            public EmptyShoppingCart(ShoppingCart emptyCart)
            {
                EmptyCart = emptyCart;
            }
        }

        public record UnvalidatedShoppingCart : IShoppingCart
        {
            public IReadOnlyCollection<UnvalidatedProduct> ProductsList { get; }
            public UnvalidatedShoppingCart(IReadOnlyCollection<UnvalidatedProduct> productsList)
            {
                ProductsList = productsList;
            }
        }

        public record InvalidatedShoppingCart : IShoppingCart
        {
            public IReadOnlyCollection<UnvalidatedProduct> ProductsList { get; }
            public string Reason { get; }
            public InvalidatedShoppingCart(IReadOnlyCollection<UnvalidatedProduct> productsList, string reason)
            {
                ProductsList = productsList;
                Reason = reason;
            }
        }

        public record FailedShoppingCart : IShoppingCart
        {
            internal FailedShoppingCart(IReadOnlyCollection<UnvalidatedProduct> productsList, Exception exception)
            {
                ProductsList = productsList;
                Exception = exception;
            }

            public IReadOnlyCollection<UnvalidatedProduct> ProductsList { get; }
            public Exception Exception { get; }
        }

        public record ValidatedShoppingCart : IShoppingCart
        {
            public IReadOnlyCollection<ValidatedProduct> ProductsList { get; }
            public ValidatedShoppingCart(IReadOnlyCollection<ValidatedProduct> productsList)
            {
                ProductsList = productsList;
            }
        }

        public record CalculatedShoppingCart : IShoppingCart
        {
            public IReadOnlyCollection<CalculatedPrice> ProductsList { get; }
            public CalculatedShoppingCart(IReadOnlyCollection<CalculatedPrice> productsList)
            {
                ProductsList = productsList;
            }
        }

        public record PaidShoppingCart : IShoppingCart
        {
            public IReadOnlyCollection<CalculatedPrice> ProductsList { get; }
            public string Csv { get; }
            public DateTime PayDate { get; }
            public PaidShoppingCart(IReadOnlyCollection<CalculatedPrice> productsList, string csv, DateTime payDate)
            {
                ProductsList = productsList;
                Csv = csv;
                PayDate = payDate;
            }
        }
    }
}
