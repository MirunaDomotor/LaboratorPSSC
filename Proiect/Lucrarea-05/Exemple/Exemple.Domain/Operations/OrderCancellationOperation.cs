//using LanguageExt;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static Exemple.Domain.Choices.OrderCancellationChoice;

//namespace Exemple.Domain.Operations
//{
//    public class OrderCancellationOperation
//    {
//        public static Task<IOrderCancellation> ValidateOrderCancellation(bool existingOrder, Func<string, Option<bool>> checkOrderStatus, UnvalidatedOrderCancellation orderCancellation) =>
//        orderCancellation
//            .Select(ValidateOrderCancel(existingOrder, checkOrderStatus))
//            .MatchAsync(
//                RightAsync: async validatedOrderCancellation => new ValidatedOrderCancellation(validatedOrderCancellation.OrderId, validatedOrderCancellation.OrderStatus),
//                LeftAsync: errorMessage => Task.FromResult((IOrderCancellation)new InvalidatedOrderCancellation(orderCancellation.OrderId, errorMessage))
//            );

//        private static Func<UnvalidatedOrderCancellation, EitherAsync<string, ValidatedOrderCancellation>> ValidateOrderCancel(bool existingOrder, Func<string, Option<bool>> checkOrderStatus) =>
//            unvalidatedOrderCancellation =>
//                from orderId in Int32.TryParse(unvalidatedOrderCancellation.OrderId)
//                                      .ToEitherAsync(() => $"Invalid order ID {unvalidatedOrderCancellation.OrderId}")
//                let orderStatusValidation = checkOrderStatus(unvalidatedOrderCancellation.OrderStatus)
//                                            .ToEitherAsync(() => $"Invalid order status {unvalidatedOrderCancellation.OrderStatus}")
//                from validatedOrderCancellation in orderStatusValidation
//                                                  .Where(_ => existingOrder && _.Value)
//                                                  .MapAsync(_ => new ValidatedOrderCancellation(orderId.Value, unvalidatedOrderCancellation.OrderStatus))
//                select validatedOrderCancellation;

//    }
//}
