using CSharp.Choices;
using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Choices
{
    [AsChoice]
    public static partial class OrderModificationChoice
    {
        public interface IOrderModification { }

        public record UnvalidatedOrderModification : IOrderModification
        {
            public IReadOnlyCollection<UnvalidatedProduct> ProductsList { get; }
            public UnvalidatedOrderModification(IReadOnlyCollection<UnvalidatedProduct> productsList)
            {
                ProductsList = productsList;
            }
        }

        public record InvalidatedOrderModification : IOrderModification
        {
            public IReadOnlyCollection<UnvalidatedProduct> ProductsList { get; }
            public string Reason { get; }
            public InvalidatedOrderModification(IReadOnlyCollection<UnvalidatedProduct> productsList, string reason)
            {
                ProductsList = productsList;
                Reason = reason;
            }
        }

        public record FailedOrderModification : IOrderModification
        {
            internal FailedOrderModification(IReadOnlyCollection<UnvalidatedProduct> productsList, Exception exception)
            {
                ProductsList = productsList;
                Exception = exception;
            }

            public IReadOnlyCollection<UnvalidatedProduct> ProductsList { get; }
            public Exception Exception { get; }
        }

        public record ValidatedOrderModification : IOrderModification
        {
            public IReadOnlyCollection<ValidatedProduct> ProductsList { get; }
            public ValidatedOrderModification(IReadOnlyCollection<ValidatedProduct> productsList)
            {
                ProductsList = productsList;
            }
        }
    }
}
