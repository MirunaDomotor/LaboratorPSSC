using System;
using Laborator4_PSCC.Domain.Models;
using static Laborator4_PSCC.Domain.Models.OrderProcessingEvent;
using static Laborator4_PSCC.Domain.ShoppingCartChoice;
using static Laborator4_PSCC.Domain.ShoppingCartOperation;
using LanguageExt;
using System.Threading.Tasks;

namespace Laborator4_PSCC.Domain
{
    public class PayShoppingCartWorkflow
    {
        public async Task<IOrderProcessingEvent> ExecuteAsync(ProcessOrderCommand command, Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductQuantity, TryAsync<bool>> checkIfEnoughStock)
        {
            UnvalidatedShoppingCart unvalidatedCart = new UnvalidatedShoppingCart(command.InputShoppingCart);
            IShoppingCart cart = await ValidateShoppingCart(checkProductExists, checkIfEnoughStock, unvalidatedCart);
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

