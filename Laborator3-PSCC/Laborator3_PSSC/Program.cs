using Laborator3_PSCC.Domain;
using System;
using System.Collections.Generic;
using static Laborator3_PSCC.Domain.ShoppingCartChoice;
using Laborator3_PSCC.Domain.Models;

namespace Laborator3_PSCC
{
    class Program
    {
        static void Main(string[] args)
        {
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

            ShoppingCart newShoppingCart = new()
            {
                Client = client,
                Products = new List<Product>()
            };

            var listOfProducts = ReadListOfProducts().ToArray();
            ProcessOrderCommand command = new(listOfProducts);
            PayShoppingCartWorkflow workflow = new PayShoppingCartWorkflow();

            var result = workflow.Execute(command, (productCode) => true, (quantity) => true);

            result.Match(
                    whenOrderProcessingFailedEvent: @event =>
                    {
                        Console.WriteLine($"Failed payment: {@event.Reason}");
                        return @event;
                    },
                    whenOrderProcessingSucceededEvent: @event =>
                    {
                        Console.WriteLine($"Successful payment: ");
                        Console.WriteLine(@event.Csv);
                        return @event;
                    }
                );
        }

        private static List<UnvalidatedProduct> ReadListOfProducts()
        {
            List<UnvalidatedProduct> listOfProducts = new();
            string response;
            do
            {
                var productCode = ReadValue("Code product (6 digits): ");
                if (string.IsNullOrEmpty(productCode))
                {
                    break;
                }

                var productQuantity = ReadValue("Quantity of product: ");
                if (string.IsNullOrEmpty(productQuantity))
                {
                    break;
                }

                int productQuantityInt = Int32.Parse(productQuantity);
                listOfProducts.Add(new(productCode, productQuantityInt));
            } while (true);

            return listOfProducts;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
