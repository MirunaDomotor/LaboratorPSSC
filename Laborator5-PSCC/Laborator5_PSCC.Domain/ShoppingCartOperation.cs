using Laborator5_PSSC.Domain.Models;
using System.Text;
using LanguageExt;
using static LanguageExt.Prelude;
using static Laborator5_PSSC.Domain.ShoppingCartChoice;

namespace Laborator5_PSSC.Domain
{
    public static class ShoppingCartOperation
    {
        public static Task<IShoppingCart> ValidateShoppingCart(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<ProductQuantity, Option<ProductQuantity>> checkEnoughStock, UnvalidatedShoppingCart shoppingCart) =>
           shoppingCart.ProductsList
                     .Select(ValidateProduct(checkProductExists, checkEnoughStock))
                     .Aggregate(CreateEmptyValidatedProductsList().ToAsync(), ReduceValidProducts)
                     .MatchAsync(
                           Right: validatedProducts => new ValidatedShoppingCart(validatedProducts),
                           LeftAsync: errorMessage => Task.FromResult((IShoppingCart)new InvalidatedShoppingCart(shoppingCart.ProductsList, errorMessage))
                     );

        private static Func<UnvalidatedProduct, EitherAsync<string, ValidatedProduct>> ValidateProduct(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<ProductQuantity, Option<ProductQuantity>> checkEnoughStock) =>
            unvalidatedProduct => ValidateProduct(checkProductExists, checkEnoughStock, unvalidatedProduct);

        private static EitherAsync<string, ValidatedProduct> ValidateProduct(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<ProductQuantity, Option<ProductQuantity>> checkEnoughStock, UnvalidatedProduct unvalidatedProduct) =>
            from productCode in ProductCode.TryParse(unvalidatedProduct.Code)
                                   .ToEitherAsync(() => $"Invalid product code {unvalidatedProduct.Code}")
            from productQuantity in ProductQuantity.TryParse(unvalidatedProduct.Quantity.ToString())
                                   .ToEitherAsync(() => $"Invalid product quantity ({unvalidatedProduct.Code}, {unvalidatedProduct.Quantity})")
            from productExists in checkProductExists(productCode)
                                   .ToEitherAsync($"Product {productCode.Value} does not exist.")
            from enoughStock in checkEnoughStock(productQuantity)
                                   .ToEitherAsync($"Product {productCode.Value} is not on stock.")
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
            whenFailedShoppingCart: failedCart => failedCart,
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
            whenFailedShoppingCart : failedCart => failedCart,
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
