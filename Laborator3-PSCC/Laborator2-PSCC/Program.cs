using Laborator2_PSCC.Domain;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using static Laborator2_PSCC.Domain.ShoppingCart;
using System.Security.AccessControl;

namespace Laborator2_PSCC
{
    class Program
    {
        private static readonly Random random = new Random();
        static void Main(string[] args)
        {
            var account = ReadCartContent();
            UnvalidatedCart unvalidatedCart = (UnvalidatedCart)account.Cart;
            IShoppingCart result = ValidateCart(unvalidatedCart);
            result.Match(
                whenEmptyCart: emptyCart => 
                {
                    Console.WriteLine("The cart is empty!");
                    return emptyCart;
                },
                whenUnvalidatedCart: unvalidatedCart =>
                {
                    Console.WriteLine("The cart is unvalidated!");
                    return unvalidatedCart;
                },
                whenInvalidatedCart: invalidatedCart =>
                {
                    Console.WriteLine("The cart is invalidated!");
                    return invalidatedCart;
                },
                whenValidatedCart: validatedCart => 
                {
                    Console.WriteLine("The cart is validated!");
                    return PaidCart(validatedCart, account.PaymentInfo);
                },
                whenPaidCart: paidCart => 
                {
                    Console.WriteLine("The cart is paid!");
                    return paidCart; 
                }
            );
        }

        private static Account ReadCartContent()
        {
            var quantityOfProducts = 0;
            List<UnvalidatedProducts> listOfProducts = new();
            var clientName = ReadValue("Client name: ");
            while (string.IsNullOrEmpty(clientName))
            {
                Console.WriteLine("Client name cannot be empty!");
                clientName = ReadValue("Please enter client name: ");
            }
            var clientAddress = ReadValue("Client address: ");
            while (string.IsNullOrEmpty(clientAddress))
            {
                Console.WriteLine("Client address cannot be empty!");
                clientAddress = ReadValue("Please enter client address: ");
            }
            Client client = new(clientName,clientAddress);
            var paymentMethod = ReadValue("Payment method (cash/card): ");
            Regex ValidPattern = new("^[0-9]{6}$");
            do
            {
                var codeProduct = ReadValue("Code product (6 digits): ");
                if (string.IsNullOrEmpty(codeProduct))
                {
                    break;
                }
                var quantityProduct = ReadValue("Quantity of product: ");
                int quantityProductInt = 0;
                if (string.IsNullOrEmpty(codeProduct))
                {
                    break;
                }
                if (!(int.TryParse(quantityProduct, out quantityProductInt)))
                {
                    break;
                }
                /* while (!(ValidPattern.IsMatch(codeProduct)))
                 {
                     Console.WriteLine("Invalid code! It must contain exactly 6 digits!");
                     codeProduct = ReadValue("Please enter a correctly formatted code product: ");
                 }
                */
                Product product = new Product(codeProduct, quantityProductInt);
                listOfProducts.Add(new (product));
            } while (true);
            decimal payAmount = 0;
            foreach (var p in listOfProducts)
            {
                payAmount = payAmount + p.Product.Price;

            }
            PaymentInfo paymentInfo = new(paymentMethod, payAmount);
            UnvalidatedCart shoppingCart = new UnvalidatedCart(listOfProducts);
            Account account = new Account(client, shoppingCart, paymentInfo);
            return account;
        }

        private static IShoppingCart ValidateCart(UnvalidatedCart unvalidatedCart)
        {
            if (unvalidatedCart.ProductsList.Count == 0)
            {
                return new EmptyCart();
            }
            else
            {
                Regex ValidPattern = new("^[0-9]{6}$");
                foreach (var product in unvalidatedCart.ProductsList)
                {
                    if (!(ValidPattern.IsMatch(product.Product.Code)))
                    {
                        return new InvalidatedCart(new List<UnvalidatedProducts>(), "Invalid code!");
                    }
                }
                return new ValidatedCart(new List<ValidatedProducts>());
            }
        }

        private static IShoppingCart PaidCart(ValidatedCart validatedCart, PaymentInfo paymentInfo)
        {
            Regex ValidPattern1 = new("^card$");
            Regex ValidPattern2 = new("^cash$");
            if (ValidPattern1.IsMatch(paymentInfo.PaymentMethod) || ValidPattern2.IsMatch(paymentInfo.PaymentMethod))
            {
                Console.WriteLine("The cart is paid!");
                Console.WriteLine(paymentInfo.ToString());
                return new PaidCart(new List<ValidatedProducts>(), paymentInfo);
            }
            else
            {
                return validatedCart;
            }
        }
        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
