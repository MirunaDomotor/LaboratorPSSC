using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using Example.Events.ServiceBus;
using Example.Events;

namespace Example.Accomodation.EventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddServiceBusClient(hostContext.Configuration.GetConnectionString("ServiceBus"));
                });

                services.AddSingleton<EventHandlersManager>();

                services.AddSingleton<IEventHandler, ProductsPaidEventHandler>();
                services.AddSingleton<IEventListener, ServiceBusTopicEventListener>();


                var serviceProvider = services.BuildServiceProvider();
                //var eventHandlersManager = serviceProvider.GetRequiredService<EventHandlersManager>();

                //var handlerEv = serviceProvider.GetRequiredService<IEventHandler>();
                //foreach (var eventType in handlerEv.EventTypes)
                //{
                //    eventHandlersManager.RegisterHandler(eventType, handlerEv.HandleAsync);
                //}
                //// Verificare handler
                //if (eventHandlersManager.TryGetHandler("ProductsPublishedEvent", out var handler))
                //{
                //    Console.WriteLine("Handler-ul a fost găsit!");
                //}
                //else
                //{
                //    Console.WriteLine("Handler-ul nu a fost găsit!");
                //}

                services.AddHostedService<Worker>();
            });
    }
}
