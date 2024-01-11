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
    public static partial class OrderCancellationChoice
    {
        public interface IOrderCancellation { }

        public record UnvalidatedOrderCancellation : IOrderCancellation
        {
            public int OrderId { get; }
            public string OrderStatus { get; }
            public UnvalidatedOrderCancellation(int orderId, string orderStatus)
            {
                OrderId = orderId;
                OrderStatus = orderStatus;
            }
        }

        public record InvalidatedOrderCancellation : IOrderCancellation
        {
            public int OrderId { get; }
            public string Reason { get; }
            public InvalidatedOrderCancellation(int orderId, string reason)
            {
                OrderId = orderId;
                Reason = reason;
            }
        }

        public record FailedOrderCancellation : IOrderCancellation
        {
            public int OrderId { get; }
            public Exception Exception { get; }
            internal FailedOrderCancellation(int orderId, Exception exception)
            {
                OrderId = orderId;
                Exception = exception;
            }
        }

        public record ValidatedOrderCancellation : IOrderCancellation
        {
            public int OrderId { get; }
            public string OrderStatus { get; }
            public ValidatedOrderCancellation(int orderId, string orderStatus)
            {
                OrderId = orderId;
                OrderStatus = orderStatus;
            }
        }
    }
}
