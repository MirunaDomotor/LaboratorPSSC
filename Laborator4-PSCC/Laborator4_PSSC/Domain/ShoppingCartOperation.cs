using Laborator4_PSCC.Domain.Models;
using Laborator3_PSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using static Laborator4_PSCC.Domain.ShoppingCartChoice;

namespace Laborator4_PSCC.Domain
{
    public static class ShoppingCartOperation
    {
        public static Task<IShoppingCart> ValidateShoppingCart(Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductQuantity, TryAsync<bool>> checkIfEnoughStock, UnvalidatedShoppingCart shoppingCart) =>
           shoppingCart.ProductsList
                     .Select(ValidateProduct(checkProductExists, checkIfEnoughStock))
                     .Aggregate(CreateEmptyValidatedProductsList().ToAsync(), ReduceValidProducts)
                     .MatchAsync(
                           Right: validatedProducts => new ValidatedShoppingCart(validatedProducts),
                           LeftAsync: errorMessage => Task.FromResult((IShoppingCart)new InvalidatedShoppingCart(shoppingCart.ProductsList, errorMessage))
                     );

        private static Func<UnvalidatedProduct, EitherAsync<string, ValidatedProduct>> ValidateProduct(Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductQuantity, TryAsync<bool>> checkIfEnoughStock) =>
            unvalidatedProduct => ValidateProduct(checkProductExists, checkIfEnoughStock, unvalidatedProduct);

        private static EitherAsync<string, ValidatedProduct> ValidateProduct(Func<ProductCode, TryAsync<bool>> checkProductExists, Func<ProductQuantity, TryAsync<bool>> checkIfEnoughStock, UnvalidatedProduct unvalidatedProduct) =>
            from productCode in ProductCode.TryParse(unvalidatedProduct.Code)
                                   .ToEitherAsync(() => $"Invalid product code {unvalidatedProduct.Code}")
            from productQuantity in ProductQuantity.TryParse(unvalidatedProduct.Quantity)
                                   .ToEitherAsync(() => $"Invalid product quantity ({unvalidatedProduct.Code}, {unvalidatedProduct.Quantity})")
            from productExists in checkProductExists(productCode)
                                   .ToEither(error => error.ToString())
            from enoughStock in checkIfEnoughStock(productQuantity)
                                   .ToEither(error => error.ToString())
            select new ValidatedProduct(productCode, productQuantity, new ProductPrice());

        private static Either<string, List<ValidatedProduct>> CreateEmptyValidatedProductsList() =>
            Right(new List<ValidatedProduct>());

        private static EitherAsync<string, List<ValidatedProduct>> ReduceValidProducts(EitherAsync<string, List<ValidatedProduct>> acc, EitherAsync<string, ValidatedProduct> next) =>
            from list in acc
            from nextProduct in next
            select list.AppendValidProduct(nextProduct);

        private static List<ValidatedProduct> AppendValidProduct(this List<ValidatedProduct> list, ValidatedProduct validProduct)
        {
            list.Add(validProduct);
            return list;
        }

        public static IShoppingCart CalculateTotalPrice(IShoppingCart cart) => cart.Match(
            whenEmptyShoppingCart: emptyCart => emptyCart,
            whenUnvalidatedShoppingCart: unvalidatedCart => unvalidatedCart,
            whenInvalidatedShoppingCart: invalidCart => invalidCart,
            whenCalculatedShoppingCart: calculatedCart => calculatedCart,
            whenPaidShoppingCart: paidCart => paidCart,
            whenValidatedShoppingCart: validCart =>
            {
                var calculateCart = validCart.ProductsList.Select(validCart =>
                                            new CalculatedPrice(validCart.Code,
                                                                      validCart.Quantity,
                                                                      validCart.Price,
                                                                      System.Math.Round(validCart.Quantity.ReturnQuantity() * validCart.Price.ReturnPrice(), 2)));

                return new CalculatedShoppingCart(calculateCart.ToList().AsReadOnly());
            }
        );

        public static IShoppingCart PayShoppingCart(IShoppingCart cart)
        {
            return cart.Match(
            whenEmptyShoppingCart: emptyCart => emptyCart,
            whenUnvalidatedShoppingCart: unvalidatedCart => unvalidatedCart,
            whenInvalidatedShoppingCart: invalidCart => invalidCart,
            whenValidatedShoppingCart: validatedCart => validatedCart,
            whenPaidShoppingCart: paidCart => paidCart,
            whenCalculatedShoppingCart: calculatedCart =>
            {
                StringBuilder csv = new();
                calculatedCart.ProductsList.Aggregate(csv, (export, cart) => export.AppendLine($"{cart.Code.Value}, {cart.Quantity.Value}, {cart.Price.Value}, {cart.TotalPrice}"));

                PaidShoppingCart paidShoppingCart = new(calculatedCart.ProductsList, csv.ToString(), DateTime.Now);

                return paidShoppingCart;
            });
        }
    }
}
