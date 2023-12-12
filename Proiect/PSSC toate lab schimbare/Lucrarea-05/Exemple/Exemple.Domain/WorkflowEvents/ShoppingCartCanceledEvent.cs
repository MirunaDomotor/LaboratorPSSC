using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.WorkflowEvents
{
    [AsChoice]
    public static partial class ShoppingCartCanceledEvent
    {
        public interface IShoppingCartCanceledEvent { }

        public record ShoppingCartSucceededEvent : IShoppingCartCanceledEvent
        {
            public string Csv { get; }
            public DateTime CanceledDate { get; }

            internal ShoppingCartSucceededEvent(string csv, DateTime canceledDate)
            {
                Csv = csv;
                CanceledDate = canceledDate;
            }
        }

        public record ShoppingCartFailedEvent : IShoppingCartCanceledEvent
        {
            public string Reason { get; }

            internal ShoppingCartFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
