using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Commands
{
    public record OrderCancellationCommand
    {
        public int OrderId { get; }
        public string OrderStatus { get; }
        public OrderCancellationCommand(int orderId, string orderStatus)
        {
            OrderId = orderId;
            OrderStatus = orderStatus;
        }
    }
}
