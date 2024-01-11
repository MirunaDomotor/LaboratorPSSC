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
using static Exemple.Domain.WorkflowEvents.OrderCancellationEvent;
using static Exemple.Domain.Choices.OrderCancellationChoice;

namespace Exemple.Domain.Workflows
{
    public class OrderCancellationWorkflow
    {
        private readonly IOrdersRepository ordersRepository;
        private readonly IProductsRepository productsRepository;
        private readonly ILogger<OrderCancellationWorkflow> logger;

        public OrderCancellationWorkflow(IOrdersRepository ordersRepository, IProductsRepository productsRepository, ILogger<OrderCancellationWorkflow> logger)
        {
            this.ordersRepository = ordersRepository;
            this.productsRepository = productsRepository;
            this.logger = logger;
        }

        public async Task<IOrderCancellationEvent> CancelOrderAsync(OrderCancellationCommand command)
        {
            UnvalidatedOrderCancellation unvalidatedOrderCancellation = new UnvalidatedOrderCancellation(command.OrderId, command.OrderStatus);

            var result = from existingOrder in ordersRepository.TryOrderExists(unvalidatedOrderCancellation.OrderId)
                                   .ToEither(ex => new FailedOrderCancellation(unvalidatedOrderCancellation.OrderId, ex) as IOrderCancellation)
                         let checkOrderStatus = (Func<string, Option<bool>>)(status => CheckOrderStatus(unvalidatedOrderCancellation.OrderStatus, status))
                         from canceledOrder in CancelOrderWorkflowAsync(unvalidatedOrderCancellation, existingOrder, checkOrderStatus).ToAsync()
                         from _ in ordersRepository.TryCancelOrder(canceledOrder.OrderId)
                                           .ToEither(ex => new FailedOrderCancellation(command.OrderId, ex) as IOrderCancellation)
                         select canceledOrder;

            return await result.Match(
                    Left: failedOrderCancellation => GenerateFailedOrderCancellationEvent(failedOrderCancellation) as IOrderCancellationEvent,
                    Right: canceledOrder => new OrderCancellationSucceededEvent("Canceled order!")
                );
        }

        private Task<Either<IOrderCancellation, ValidatedOrderCancellation>> CancelOrderWorkflowAsync(UnvalidatedOrderCancellation orderCancellation,
                                                                                          bool existingOrder,
                                                                                          Func<string, Option<bool>> checkOrderStatus)
        {
            IOrderCancellation order;
            if (existingOrder && checkOrderStatus("processed").IsSome)
            {
                order = new ValidatedOrderCancellation(orderCancellation.OrderId, orderCancellation.OrderStatus);
            }
            else
            {
                order = new InvalidatedOrderCancellation(orderCancellation.OrderId, orderCancellation.OrderStatus);
            }

            return Task.FromResult(order.Match<Either<IOrderCancellation, ValidatedOrderCancellation>>(
                whenUnvalidatedOrderCancellation: unvalidatedOrderCancellation => Left(unvalidatedOrderCancellation as IOrderCancellation),
                whenInvalidatedOrderCancellation: invalidatedOrderCancellation => Left(invalidatedOrderCancellation as IOrderCancellation),
                whenFailedOrderCancellation: failedOrderCancellation => Left(failedOrderCancellation as IOrderCancellation),
                whenValidatedOrderCancellation: validatedOrderCancellation => Right(validatedOrderCancellation)
            ));
        }

        private Option<bool> CheckOrderStatus(string orderStatus, string status)
        {
            if (orderStatus == status)
            {
                return Some(true);
            }
            else
            {
                return Some(false);
            }
        }

        private OrderCancellationFailedEvent GenerateFailedOrderCancellationEvent(IOrderCancellation orderCancellation) =>
                orderCancellation.Match<OrderCancellationFailedEvent>(
                    whenUnvalidatedOrderCancellation: unvalidatedOrderCancellation => new($"Invalid state {nameof(UnvalidatedOrderCancellation)}"),
                    whenInvalidatedOrderCancellation: invalidatedOrderCancellation => new(invalidatedOrderCancellation.Reason),
                    whenValidatedOrderCancellation: validatedOrderCancellation => new($"Invalid state {nameof(ValidatedShoppingCart)}"),
                    whenFailedOrderCancellation: failedOrderCancellation =>
                    {
                        logger.LogError(failedOrderCancellation.Exception, failedOrderCancellation.Exception.Message);
                        return new(failedOrderCancellation.Exception.Message);
                    });

    }
}
