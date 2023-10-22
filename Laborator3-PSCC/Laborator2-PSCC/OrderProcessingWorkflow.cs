using Laborator2_PSCC.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator2_PSCC.Domain.OrderProcessingEvent;
using static Laborator2_PSCC.Domain.ShoppingCart;
using static Laborator2_PSCC.ProductsOperation;

namespace Laborator2_PSCC
{
    public class OrderProcessingWorkflow
    {
        public IOrderProcessingEvent Execute(ProcessOrderCommand command, Func<Product, bool> checkProductExists)
        {
            UnvalidatedCart unvalidatedProducts = new UnvalidatedCart(command.InputProducts);
            IShoppingCart cart = ValidateProducts(checkProductExists, unvalidatedProducts);
            cart = ProcessCart(cart);
            return cart.Match(
                    whenEmptyCart: emptyCart => new OrderProcessingFailedEvent("Empty cart") as IOrderProcessingEvent,
                    whenUnvalidatedCart: unvalidatedCart => new OrderProcessingFailedEvent("Unexpected unvalidated state"),
                    whenInvalidatedCart: invalidCart => new OrderProcessingFailedEvent(invalidCart.Reason),
                    whenValidatedCart: validatedCart => new OrderProcessingFailedEvent("Unexpected validated state"),
                    whenPaidCart: publishedCart => new OrderProcessingSucceededEvent(publishedCart.Csv, publishedCart.PublishedDate)
                );
        }
    }
}
