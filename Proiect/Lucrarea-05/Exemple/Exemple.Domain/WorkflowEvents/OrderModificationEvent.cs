using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.WorkflowEvents.OrderModificationEvent;

namespace Exemple.Domain.WorkflowEvents
{
    [AsChoice]
    public static partial class OrderModificationEvent
    {
        public interface IOrderModificationEvent { }

        public record OrderModificationSucceededEvent : IOrderModificationEvent
        {
            public string Csv { get; }
            public DateTime ModificationDate { get; }

            internal OrderModificationSucceededEvent(string csv, DateTime modificationDate)
            {
                Csv = csv;
                ModificationDate = modificationDate;
            }
        }

        public record OrderModificationFailedEvent : IOrderModificationEvent
        {
            public string Reason { get; }

            internal OrderModificationFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
