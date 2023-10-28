using Laborator3_PSCC.Domain.Models;
using Laborator3_PSSC.Domain.Models;
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
        public static IShoppingCart ValidateShoppingCart(Func<ProductCode, bool> checkProductExists, Func<ProductQuantity, bool> checkIfEnoughStock, UnvalidatedShoppingCart shoppingCart)
        {
            List<ValidatedProduct> validatedCart = new();
            bool isValidList = true;
            string invalidReason = string.Empty;

            foreach (var unvalidatedCart in shoppingCart.ProductsList)
            {
                if(!ProductCode.TryParse(unvalidatedCart.Code, out ProductCode? codeValidation) || !checkProductExists(codeValidation))
                {
                    invalidReason = $"Invalid product code {unvalidatedCart.Code}!";
                    isValidList = false;
                    break;
                }
                if(!ProductQuantity.TryParse(unvalidatedCart.Quantity, out ProductQuantity? quantityValidation) || !checkIfEnoughStock(quantityValidation))
                {
                    invalidReason = $"Invalid product quantity ({unvalidatedCart.Code},{unvalidatedCart.Quantity})";
                    isValidList = false;
                    break;
                }
                ProductPrice Price = new ProductPrice();
                ValidatedProduct validProduct = new(codeValidation, quantityValidation, Price);
                validatedCart.Add(validProduct);
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
