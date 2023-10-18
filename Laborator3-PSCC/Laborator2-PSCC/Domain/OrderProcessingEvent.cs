using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    [AsChoice]
    internal class OrderProcessingEvent
    {
        public interface IOrderProcessingEvent { }
        public record OrderProcessingSucceededEvent : IOrderProcessingEvent
        {
            public string Csv { get; }
            public DateTime ProcessedOrder { get; }
            internal OrderProcessingSucceededEvent(string csv, DateTime processedOrder)
            {
                Csv = csv;
                ProcessedOrder = processedOrder;
            }
        }

        public record OrderProcessingFailedEvent : IOrderProcessingEvent
        {
            public string Reason { get; }
            internal OrderProcessingFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
