using Exemple.Domain.Models;
using static LanguageExt.Prelude;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Exemple.Domain.Models.ShoppingCartChoice;
using System.Threading.Tasks;

namespace Exemple.Domain
{
    public static class ShoppingCartOperation
    {
        public static Task<IShoppingCart> ValidateShoppingCart(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<ProductQuantity, ProductCode, Option<ProductQuantity>> checkEnoughStock, UnvalidatedShoppingCart shoppingCart) =>
           shoppingCart.ProductsList
                     .Select(ValidateProduct(checkProductExists, checkEnoughStock))
                     .Aggregate(CreateEmptyValidatedProductsList().ToAsync(), ReduceValidProducts)
                     .MatchAsync(
                           Right: validatedProducts => new ValidatedShoppingCart(validatedProducts),
                           LeftAsync: errorMessage => Task.FromResult((IShoppingCart)new InvalidatedShoppingCart(shoppingCart.ProductsList, errorMessage))
                     );

        private static Func<UnvalidatedProduct, EitherAsync<string, ValidatedProduct>> ValidateProduct(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<ProductQuantity, ProductCode, Option<ProductQuantity>> checkEnoughStock) =>
            unvalidatedProduct => ValidateProduct(checkProductExists, checkEnoughStock, unvalidatedProduct);

        private static EitherAsync<string, ValidatedProduct> ValidateProduct(Func<ProductCode, Option<ProductCode>> checkProductExists, Func<ProductQuantity, ProductCode, Option<ProductQuantity>> checkEnoughStock, UnvalidatedProduct unvalidatedProduct) =>
            from productCode in ProductCode.TryParse(unvalidatedProduct.Code)
                                   .ToEitherAsync(() => $"Invalid product code {unvalidatedProduct.Code}")
            from productQuantity in ProductQuantity.TryParse(unvalidatedProduct.Quantity.ToString())
                                   .ToEitherAsync(() => $"Invalid product quantity ({unvalidatedProduct.Code}, {unvalidatedProduct.Quantity})")
            from productStock in ProductStock.TryParse(unvalidatedProduct.Stock.ToString())
                             .ToEitherAsync(() => $"Invalid product stock ({unvalidatedProduct.Code}, {unvalidatedProduct.Stock})")
            from productPrice in ProductPrice.TryParse(unvalidatedProduct.Price.ToString())
                        .ToEitherAsync(() => $"Invalid product price ({unvalidatedProduct.Code}, {unvalidatedProduct.Price})")
            from productExists in checkProductExists(productCode)
                                   .ToEitherAsync($"Product {productCode.Value} does not exist.")
            from enoughStock in checkEnoughStock(productQuantity, productCode)
                                   .ToEitherAsync($"Product {productCode.Value} is not on stock.")
            select new ValidatedProduct(new(productCode, productQuantity, productStock, productPrice));

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
            whenValidatedShoppingCart: CalculateTotal);

        private static IShoppingCart CalculateTotal(ValidatedShoppingCart validExamGrades) =>
            new CalculatedShoppingCart(validExamGrades.ProductsList
                                                    .Select(CalculateProductFinalPrice)
                                                    .ToList()
                                                    .AsReadOnly());

        private static CalculatedPrice CalculateProductFinalPrice(ValidatedProduct validCart) =>
            new CalculatedPrice(validCart.Product.Code,
                                      validCart.Product.Quantity,
                                      validCart.Product.Stock,
                                      validCart.Product.Price,
                                      System.Math.Round(validCart.Product.Quantity.ReturnQuantity() * validCart.Product.Price.ReturnPrice(), 2));

        public static IShoppingCart MergeProducts(IShoppingCart cart, IEnumerable<CalculatedPrice> existingProducts) => cart.Match(
            whenEmptyShoppingCart: emptyCart => emptyCart,
            whenUnvalidatedShoppingCart: unvalidatedCart => unvalidatedCart,
            whenInvalidatedShoppingCart: invalidCart => invalidCart,
            whenFailedShoppingCart: failedCart => failedCart,
            whenValidatedShoppingCart: validatedCart => validatedCart,
            whenPaidShoppingCart: paidCart => paidCart,
            whenCalculatedShoppingCart: calculatedCart => MergeProducts(calculatedCart.ProductsList, existingProducts));

        private static CalculatedShoppingCart MergeProducts(IEnumerable<CalculatedPrice> newList, IEnumerable<CalculatedPrice> existingList)
        {
            var updatedAndNewProducts = newList.Select(product => product with { OrderLineId = existingList.FirstOrDefault(p => p.Code == product.Code)?.OrderLineId ?? 0, IsUpdated = true });
            var oldProducts = existingList.Where(product => !newList.Any(p => p.Code == product.Code));
            var allProducts = updatedAndNewProducts.Union(oldProducts)
                                               .ToList()
                                               .AsReadOnly();
            return new CalculatedShoppingCart(allProducts);

        }


            public static IShoppingCart PayShoppingCart(IShoppingCart cart)
            => cart.Match(
            whenEmptyShoppingCart: emptyCart => emptyCart,
            whenUnvalidatedShoppingCart: unvalidatedCart => unvalidatedCart,
            whenInvalidatedShoppingCart: invalidCart => invalidCart,
            whenFailedShoppingCart: failedCart => failedCart,
            whenValidatedShoppingCart: validatedCart => validatedCart,
            whenPaidShoppingCart: paidCart => paidCart,
            whenCalculatedShoppingCart: GenerateExport);


        private static IShoppingCart GenerateExport(CalculatedShoppingCart calculatedCart) =>
          new PaidShoppingCart(calculatedCart.ProductsList,
                                  calculatedCart.ProductsList.Aggregate(new StringBuilder(), CreateCsvLine).ToString(),
                                  DateTime.Now);

        private static StringBuilder CreateCsvLine(StringBuilder export, CalculatedPrice cart) =>
            export.AppendLine($"{cart.Code.Value}, {cart.Quantity.Value}, {cart.Stock.Stock}, {cart.Price.Value}, {cart.TotalPrice}");
    }
}
