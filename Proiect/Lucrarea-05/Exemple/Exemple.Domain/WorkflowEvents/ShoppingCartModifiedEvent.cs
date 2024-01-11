using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Exemple.Domain.WorkflowEvents.ShoppingCartModifiedEvent;

namespace Exemple.Domain.WorkflowEvents
{
    [AsChoice]
    public static partial class ShoppingCartModifiedEvent
    {
        public interface IShoppingCartModifiedEvent { }

        public record ShoppingCartSucceededEvent : IShoppingCartModifiedEvent
        {
            public string Csv { get; }
            public DateTime ModifiedDate { get; }

            internal ShoppingCartSucceededEvent(string csv, DateTime modifiedDate)
            {
                Csv = csv;
                ModifiedDate = modifiedDate;
            }
        }

        public record ShoppingCartFailedEvent : IShoppingCartModifiedEvent
        {
            public string Reason { get; }

            internal ShoppingCartFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
