using Laborator2_PSCC.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator2_PSCC.Domain.OrderProcessingEvent;
using static Laborator2_PSCC.Domain.ShoppingCart;

namespace Laborator2_PSCC
{
    public class OrderProcessingWorkflow
    {
        public IOrderProcessingEvent Execute(ProcessOrderCommand command, Func<, bool> checkProductExists)
        {
            UnvalidatedProducts unvalidatedProducts = new UnvalidatedProducts(command.InputProducts);
            IShoppingCart cart = ValidateProducts(checkProductExists, unvalidatedProducts);
            cart = ProcessCart(cart);
            return cart.Match(
                    whenUnvalidatedCart: unvalidatedCart => new OrderProcessingFailedEvent("Unexpected unvalidated state") as IOrderProcessingEvent,
                    whenInvalidatedCart: invalidCart => new OrderProcessingFailedEvent(invalidCart.Reason),
                    whenValidatedCart: validatedCart => new OrderProcessingFailedEvent("Unexpected validated state"),
                    whenCalculatedCart: calculatedCart => new OrderProcessingFailedEvent("Unexpected calculated state"),
                    whenPaidCart: publishedCart => new OrderProcessingSucceededEvent(publishedCart.Csv, publishedCart.PublishedDate)
                );
        }
    }
}
