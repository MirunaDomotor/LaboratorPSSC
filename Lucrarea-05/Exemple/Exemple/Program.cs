using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using static Exemple.Domain.Models.ShoppingCartChoice;
using Exemple.Domain;
using System.Threading.Tasks;
using LanguageExt;
using static LanguageExt.Prelude;
using Example.Data.Repositories;
using Example.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Exemple
{
    class Program
    {
        private static readonly Regex ValidPatternName = new("^[a-zA-Z]{4,20}$");
        private static readonly Regex ValidPatternAddress = new("^[a-zA-Z][a-zA-Z0-9]*$");

        private static string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=Product;Trusted_Connection=True;";

        static async Task Main(string[] args)
        {
            using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
            ILogger<PayShoppingCartWorkflow> logger = loggerFactory.CreateLogger<PayShoppingCartWorkflow>();

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
            Client client = new(clientName, clientAddress);

            ShoppingCart newShoppingCart = new()
            {
                Client = client,
                Products = new List<Product>()
            };

            var listOfProducts = ReadListOfProducts().ToArray();
            ProcessOrderCommand command = new(listOfProducts);

            var dbContextBuilder = new DbContextOptionsBuilder<ProductsContext>()
                                                .UseSqlServer(ConnectionString)
                                                .UseLoggerFactory(loggerFactory);
            ProductsContext productsContext = new ProductsContext(dbContextBuilder.Options);
            OrdersRepository ordersRepository = new(productsContext);
            ProductsRepository productsRepository = new(productsContext);
            PayShoppingCartWorkflow workflow = new(ordersRepository, productsRepository, logger);

            //PayShoppingCartWorkflow workflow = new PayShoppingCartWorkflow(); 
            var result = await workflow.ExecuteAsync(command);

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

        private static ILoggerFactory ConfigureLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
                                builder.AddSimpleConsole(options =>
                                {
                                    options.IncludeScopes = true;
                                    options.SingleLine = true;
                                    options.TimestampFormat = "hh:mm:ss ";
                                })
                                .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
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

                listOfProducts.Add(new(productCode, int.Parse(productQuantity), 35));
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
