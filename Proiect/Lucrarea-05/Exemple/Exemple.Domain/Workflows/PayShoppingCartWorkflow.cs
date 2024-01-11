using Exemple.Domain.Models;
using static Exemple.Domain.ShoppingCartOperation;
using System;
using static Exemple.Domain.Models.ShoppingCartChoice;
using LanguageExt;
using System.Threading.Tasks;
using System.Collections.Generic;
using Exemple.Domain.Repositories;
using System.Linq;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Logging;
using static Exemple.Domain.Models.OrderProcessingEvent;
using Exemple.Domain.Commands;

namespace Exemple.Domain
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
                         from existingProducts in ordersRepository.TryGetExistingProducts()
                                          .ToEither(ex => new FailedShoppingCart(unvalidatedCart.ProductsList, ex) as IShoppingCart)
                         let checkProductExists = (Func<ProductCode, Option<ProductCode>>)(productCode => CheckProductExists(products, productCode))
                         from existingCart in productsRepository.TryGetStockProduct(products.Value)
                                          .ToEither(ex => new FailedShoppingCart(unvalidatedCart.ProductsList, ex) as IShoppingCart)
                         let checkEnoughStock = (Func<ProductQuantity,ProductCode, Option<ProductQuantity>>)((quantity, productCode) => CheckEnoughStock(existingCart, quantity))
                         from paidCart in ExecuteWorkflowAsync(unvalidatedCart, existingProducts, checkProductExists, checkEnoughStock).ToAsync()
                         from _ in ordersRepository.TrySaveOrder(paidCart)
                                          .ToEither(ex => new FailedShoppingCart(unvalidatedCart.ProductsList, ex) as IShoppingCart)
                         select paidCart;

            return await result.Match(
                    Left: products => GenerateFailedEvent(products) as IOrderProcessingEvent,
                    Right: paidCart => new OrderProcessingSucceededEvent(paidCart.Csv, paidCart.PayDate)
                );
        }

        private async Task<Either<IShoppingCart, PaidShoppingCart>> ExecuteWorkflowAsync(UnvalidatedShoppingCart unvalidatedCart,
                                                                                          IEnumerable<CalculatedPrice> existingProducts,
                                                                                          Func<ProductCode, Option<ProductCode>> checkProductExists,
                                                                                          Func<ProductQuantity, ProductCode, Option<ProductQuantity>> checkEnoughStock)
        {

            IShoppingCart cart = await ValidateShoppingCart(checkProductExists, checkEnoughStock, unvalidatedCart);
            cart = CalculateTotalPrice(cart);
            cart = MergeProducts(cart, existingProducts);
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

        private Option<ProductCode> CheckProductExists(ProductCode product, ProductCode productCode)
        {
            if (product == productCode)
            {
                return Some(productCode);
            }
            else
            {
                return None;
            }
        }

        private Option<ProductQuantity> CheckEnoughStock(ProductStock stock, ProductQuantity productQuantity)
        {
            if (stock.Stock >= productQuantity.Value) 
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
