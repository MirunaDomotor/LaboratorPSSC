using Laborator5_PSSC.Domain.Models;
using static Laborator5_PSSC.Domain.Models.OrderProcessingEvent;
using static Laborator5_PSSC.Domain.ShoppingCartChoice;
using static Laborator5_PSSC.Domain.ShoppingCartOperation;
using LanguageExt;
using Laborator5_PSCC.Domain.Repositories;
using Laborator5_PSSC.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq;
using static LanguageExt.Prelude;

namespace Laborator5_PSSC.Domain
{
    public class PayShoppingCartWorkflow
    {
        private readonly IOrdersRepository ordersRepository;
        private readonly IProductsRepository productsRepository;
        private readonly ILogger<PayShoppingCartWorkflow> logger;

        public PayShoppingCartWorkflow(IOrdersRepository ordersRepository, IProductsRepository productsRepository, ILogger<PayShoppingCartWorkflow> logger)
        {
            this.ordersRepository = ordersRepository;
            this.productsRepository = productsRepository;
            this.logger = logger;
        }

        public async Task<IOrderProcessingEvent> ExecuteAsync(ProcessOrderCommand command)
        {
            UnvalidatedShoppingCart unvalidatedCart = new UnvalidatedShoppingCart(command.InputShoppingCart);

            var result = from products in productsRepository.TryGetExistingProductCode(unvalidatedCart.ProductsList.Select(product => product.Code))
                                          .ToEither(ex => new FailedShoppingCart(unvalidatedCart.ProductsList, ex) as IShoppingCart)
                         let checkProductExists = (Func<ProductCode, Option<ProductCode>>)(product => CheckProductExists(products, product))
                         from existingCart in productsRepository.TryGetStockProduct(unvalidatedCart.ProductsList.Select(product => product.Quantity))
                                          .ToEither(ex => new FailedShoppingCart(unvalidatedCart.ProductsList, ex) as IShoppingCart)
                         let checkEnoughStock = (Func<ProductQuantity, Option<ProductQuantity>>)(quantity => CheckEnoughStock(existingCart, quantity))
                         from paidCart in ExecuteWorkflowAsync(unvalidatedCart, checkProductExists, checkEnoughStock).ToAsync()
                         from _ in ordersRepository.TrySaveOrder(paidCart)
                                          .ToEither(ex => new FailedShoppingCart(unvalidatedCart.ProductsList, ex) as IShoppingCart)
                         select paidCart;

            return await result.Match(
                    Left: products => GenerateFailedEvent(products) as IOrderProcessingEvent,
                    Right: paidCart => new OrderProcessingSucceededEvent(paidCart.Csv, paidCart.PayDate)
                );
        }

        //public async Task<IOrderProcessingEvent> ExecuteAsync(ProcessOrderCommand command, Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductQuantity, TryAsync<bool>> checkIfEnoughStock)
        //{
        //    UnvalidatedShoppingCart unvalidatedCart = new UnvalidatedShoppingCart(command.InputShoppingCart);
        //    IShoppingCart cart = await ValidateShoppingCart(checkProductExists, checkIfEnoughStock, unvalidatedCart);
        //    cart = CalculateTotalPrice(cart);
        //    cart = PayShoppingCart(cart);

        //    return cart.Match(
        //            whenEmptyShoppingCart: emptyCart => new OrderProcessingFailedEvent("Unexpected empty state") as IOrderProcessingEvent,
        //            whenUnvalidatedShoppingCart: unvalidatedCart => new OrderProcessingFailedEvent("Unexpected unvalidated state"),
        //            whenInvalidatedShoppingCart: invalidCart => new OrderProcessingFailedEvent(invalidCart.Reason),
        //            whenValidatedShoppingCart: validatedCart => new OrderProcessingFailedEvent("Unexpected validated state"),
        //            whenCalculatedShoppingCart: calculatedCart => new OrderProcessingFailedEvent("Unexpected calculated state"),
        //            whenPaidShoppingCart: paidCart => new OrderProcessingSucceededEvent(paidCart.Csv, paidCart.PayDate)
        //        );
        //}

        private async Task<Either<IShoppingCart, PaidShoppingCart>> ExecuteWorkflowAsync(UnvalidatedShoppingCart unvalidatedCart,
                                                                                          Func<ProductCode, Option<ProductCode>> checkProductExists, 
                                                                                          Func<ProductQuantity, Option<ProductQuantity>> checkEnoughStock)
        {

            IShoppingCart cart = await ValidateShoppingCart(checkProductExists, checkEnoughStock, unvalidatedCart);
            cart = CalculateTotalPrice(cart);
            cart = PayShoppingCart(cart);

            return cart.Match<Either<IShoppingCart, PaidShoppingCart>>(
                whenEmptyShoppingCart: emptyCart => Left(emptyCart as IShoppingCart),
                whenUnvalidatedShoppingCart: unvalidatedCart => Left(unvalidatedCart as IShoppingCart),
                whenInvalidatedShoppingCart: invalidCart => Left(invalidCart as IShoppingCart),
                whenValidatedShoppingCart: validatedCart => Left(validatedCart as IShoppingCart),
                whenFailedShoppingCart: failedCart => Left(failedCart as IShoppingCart),
                whenCalculatedShoppingCart: calculatedCart => Left(calculatedCart as IShoppingCart),
                whenPaidShoppingCart: paidCart => Right(paidCart)
            );
        }

        private Option<ProductCode> CheckProductExists(IEnumerable<ProductCode> products, ProductCode productCode)
        {
            if (products.Any(s => s == productCode))
            {
                return Some(productCode);
            }
            else
            {
                return None;
            }
        }

        private Option<ProductQuantity> CheckEnoughStock(IEnumerable<ProductQuantity> products, ProductQuantity productQuantity)
        {
            if (products.Any(s => s == productQuantity))
            {
                return Some(productQuantity);
            }
            else
            {
                return None;
            }
        }

        private OrderProcessingFailedEvent GenerateFailedEvent(IShoppingCart cart) =>
            cart.Match<OrderProcessingFailedEvent>(
                whenEmptyShoppingCart: emptyShoppingCart => new($"Empty state {nameof(EmptyShoppingCart)}"),
                whenUnvalidatedShoppingCart: unvalidatedShoppingCart => new($"Invalid state {nameof(UnvalidatedShoppingCart)}"),
                whenInvalidatedShoppingCart: invalidShoppingCart => new(invalidShoppingCart.Reason),
                whenValidatedShoppingCart: validatedShoppingCart => new($"Invalid state {nameof(ValidatedShoppingCart)}"),
                whenFailedShoppingCart: failedShoppingCart =>
                {
                    logger.LogError(failedShoppingCart.Exception, failedShoppingCart.Exception.Message);
                    return new(failedShoppingCart.Exception.Message);
                },
                whenCalculatedShoppingCart: calculatedShoppingCart => new($"Invalid state {nameof(CalculatedShoppingCart)}"),
                whenPaidShoppingCart: paidShoppingCart => new($"Invalid state {nameof(PaidShoppingCart)}"));
    }
}

