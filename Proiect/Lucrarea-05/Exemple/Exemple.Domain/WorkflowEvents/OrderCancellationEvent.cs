using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.WorkflowEvents
{
    [AsChoice]
    public static partial class OrderCancellationEvent
    {
        public interface IOrderCancellationEvent { }

        public record OrderCancellationSucceededEvent : IOrderCancellationEvent
        {
            public string Message { get; }

            internal OrderCancellationSucceededEvent(string message)
            {
                Message = message;
            }
        }

        public record OrderCancellationFailedEvent : IOrderCancellationEvent
        {
            public string Reason { get; }

            internal OrderCancellationFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
