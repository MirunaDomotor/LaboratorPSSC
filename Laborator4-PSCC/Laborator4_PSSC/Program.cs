using Laborator4_PSCC.Domain;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Laborator4_PSCC.Domain.ShoppingCartChoice;
using Laborator4_PSCC.Domain.Models;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Net.Http;


namespace Laborator4_PSCC
{
    class Program
    {
        private static readonly Regex ValidPatternName = new("^[a-zA-Z]{4,20}$");
        private static readonly Regex ValidPatternAddress = new("^[a-zA-Z][a-zA-Z0-9]*$");
        static async Task Main(string[] args)
        {
            var clientName = ReadValue("Client name: ");
            while (string.IsNullOrEmpty(clientName) || !ValidPatternName.IsMatch(clientName))
            {
                Console.WriteLine("Client name is empty or wrongly formatted!");
                clientName = ReadValue("Please enter client name: ");
            }
            var clientAddress = ReadValue("Client address: ");
            while (string.IsNullOrEmpty(clientAddress) || !ValidPatternAddress.IsMatch(clientAddress))
            {
                Console.WriteLine("Client address is empty or wrongly formatted!!");
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
            var result = await workflow.ExecuteAsync(command, CheckProductExists, CheckIfEnoughStock);

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

                listOfProducts.Add(new(productCode, productQuantity));
            } while (true);
            return listOfProducts;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        private static TryAsync<bool> CheckProductExists(ProductCode code)
        {
            Func<Task<bool>> func = async () =>
            {
                //HttpClient client = new HttpClient();

                //var response = await client.PostAsync($"www.university.com/checkRegistrationNumber?number={student.Value}", new StringContent(""));

                //response.EnsureSuccessStatusCode(); //200

                return true;
            };
            return TryAsync(func);
        }

        private static TryAsync<bool> CheckIfEnoughStock(ProductQuantity quantity)
        {
            Func<Task<bool>> func = async () =>
            {
                //HttpClient client = new HttpClient();

                //var response = await client.PostAsync($"www.university.com/checkRegistrationNumber?number={student.Value}", new StringContent(""));

                //response.EnsureSuccessStatusCode(); //200

                return true;
            };
            return TryAsync(func);
        }
    }
}
