using Laborator2_PSCC.Domain;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Laborator2_PSCC.Domain.ShoppingCart;

namespace Laborator2_PSCC
{
    internal class ProductsOperation
    {
        public static IShoppingCart ValidateProducts(Func<string, bool> checkProductExists, UnvalidatedCart products)
        {
            List<ValidatedProducts> validatedProducts = new();
            bool isValidList = true;
            string invalidReason = string.Empty;
            foreach (var unvalidatedProduct in products.ProductsList)
            {
                if(!Product.TryParseCode(unvalidatedProduct.Code, out string code)
                           && checkProductExists(code))
                {
                    invalidReason = $"Invalid code product ({unvalidatedProduct.Code})";
                    isValidList = false;
                    break;
                }
                if(!Product.TryParseQuantity(unvalidatedProduct.Quantity, out int quantity))
                {
                    invalidReason = $"Invalid product ({unvalidatedProduct.Code},{unvalidatedProduct.Quantity})";   
                    isValidList = false ;
                    break;
                }
                ValidatedProducts validCart = new(code, quantity,unvalidatedProduct.Price);
                validatedProducts.Add(validCart);
            }

            if (isValidList)
            {
                return new ValidatedCart(validatedProducts);
            }
            else
            {
                return new InvalidatedCart(products.ProductsList, invalidReason);
            }

        }

        public static IShoppingCart CalculateFinalPrice(IShoppingCart shoppingCart) => shoppingCart.Match(
            whenEmptyCart: emptyCart => emptyCart,
            whenUnvalidatedCart: unvalidCart => unvalidCart,
            whenInvalidatedCart: invalidCart => invalidCart,
            whenValidatedCart: validatedCart =>
            {
                var calculatedCart = validatedCart.ProductsList.Select(validProduct =>
                                           new CalculatedProducts(validProduct, validProduct.Price));
                return new CalculatedCart(calculatedCart.ToList().AsReadOnly());
            }
            whenPaidCart: paidCart => paidCart
        ); 

        public static IShoppingCart ProcessCart(IShoppingCart shoppingCart) => shoppingCart.Match(
            whenEmptyCart: emptyCart => emptyCart,
            whenUnvalidatedCart: unvalidatedCart => unvalidatedCart,
            whenInvalidatedCart: invalidCart => invalidCart,
            whenValidatedCart: validatedCart => validatedCart,
            whenPaidCart: paidCart => paidCart,
            whenCalculatedCart: calculatedCart => 
            {
                StringBuilder csv = new();
                calculatedCart.ProductsList.Aggregate(csv, (export, product) => export.AppendLine($"{product.}, {grade.ExamGrade}, {grade.ActivityGrade}, , {grade.FinalGrade}"));

                PaidCart paidCart = new(calculatedCart.ProductsList, csv.ToString(), DateTime.Now);

                return paidCart;
            });
    }
}
