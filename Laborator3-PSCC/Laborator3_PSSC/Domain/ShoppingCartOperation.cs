using Laborator3_PSCC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator3_PSCC.Domain.ShoppingCartChoice;

namespace Laborator3_PSCC.Domain
{
    public static class ShoppingCartOperation
    {
        public static IShoppingCart ValidateShoppingCart(Func<ProductCodeValidation, bool> checkProductExists, Func<ProductQuantityValidation, bool> checkIfEnoughStock, UnvalidatedShoppingCart shoppingCart)
        {
            List<ValidatedProduct> validatedCart = new();
            bool isValidList = true;
            string invalidReason = string.Empty;

            foreach (var unvalidatedCart in shoppingCart.ProductsList)
            {
                ProductCodeValidation codeValidation = new(unvalidatedCart.Code);
                ProductQuantityValidation quantityValidation = new(unvalidatedCart.Quantity);

                if (unvalidatedCart.Code == null || !checkProductExists(codeValidation))
                {
                    invalidReason = $"Invalid product code {unvalidatedCart.Code}!";
                    isValidList = false;
                    break;
                }
                if (unvalidatedCart.Quantity == 0 || !checkIfEnoughStock(quantityValidation))
                {
                    invalidReason = $"Invalid quantity {unvalidatedCart.Quantity}!";
                    isValidList = false;
                    break;
                }
                if (isValidList)
                {
                    ProductCodeValidation Code = new(unvalidatedCart.Code);
                    ProductQuantityValidation Quantity = new(unvalidatedCart.Quantity);
                    ValidatedProduct validProduct = new(Code, Quantity,Quantity.CalculateTotalPrice());
                    validatedCart.Add(validProduct);
                }
            }
            if (isValidList)
            {
                return new ValidatedShoppingCart(validatedCart);
            }
            else
            {
                return new InvalidatedShoppingCart(shoppingCart.ProductsList, invalidReason);
            }

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
                                                                      validCart.Quantity.CalculateTotalPrice()));

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
                calculatedCart.ProductsList.Aggregate(csv, (export, cart) => export.AppendLine($"{cart.Code.Value}, {cart.Quantity.Value}, {cart.Quantity.CalculateTotalPrice()}"));

                PaidShoppingCart paidShoppingCart = new(calculatedCart.ProductsList, csv.ToString(), DateTime.Now);

                return paidShoppingCart;
            });
        }
    }
}
