using System;
using Laborator3_PSCC.Domain.Models;
using static Laborator3_PSCC.Domain.Models.OrderProcessingEvent;
using static Laborator3_PSCC.Domain.ShoppingCartChoice;
using static Laborator3_PSCC.Domain.ShoppingCartOperation;

namespace Laborator3_PSCC.Domain
{
    public class PayShoppingCartWorkflow
    {
        public IOrderProcessingEvent Execute(ProcessOrderCommand command, Func<ProductCodeValidation, bool> checkProductExists, Func<ProductQuantityValidation, bool> checkIfEnoughStock)
        {
            UnvalidatedShoppingCart unvalidatedCart = new UnvalidatedShoppingCart(command.InputShoppingCart);
            IShoppingCart cart = ValidateShoppingCart(checkProductExists, checkIfEnoughStock, unvalidatedCart);
            cart = CalculateTotalPrice(cart);
            cart = PayShoppingCart(cart);

            return cart.Match(
                    whenEmptyShoppingCart: emptyCart => new OrderProcessingFailedEvent("Unexpected empty state") as IOrderProcessingEvent,
                    whenUnvalidatedShoppingCart: unvalidatedCart => new OrderProcessingFailedEvent("Unexpected unvalidated state"),
                    whenInvalidatedShoppingCart: invalidCart => new OrderProcessingFailedEvent(invalidCart.Reason),
                    whenValidatedShoppingCart: validatedCart => new OrderProcessingFailedEvent("Unexpected validated state"),
                    whenCalculatedShoppingCart: calculatedCart => new OrderProcessingFailedEvent("Unexpected calculated state"),
                    whenPaidShoppingCart: paidCart => new OrderProcessingSucceededEvent(paidCart.Csv, paidCart.PayDate)
                );
        }
    }
}

