using Exemple.Domain;
using Exemple.Domain.Models;
using Exemple.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Example.API.Models;
using Example.Data;
using Example.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using Example.Dto;
using Example.Events.ServiceBus;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using Microsoft.Extensions.Azure;
using LanguageExt.ClassInstances;
using LanguageExt.Pipes;
using Example.Data.Models;
using Exemple.Domain.Commands;
using Exemple.Domain.Workflows;
using Exemple.Domain.WorkflowEvents;

namespace Example.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private ILogger<ProductsController> logger;
        private readonly PayShoppingCartWorkflow payShoppingCartWorkflow;
        private readonly OrderCancellationWorkflow orderCancellationWorkflow;
        private readonly OrderModificationWorkflow orderModificationWorkflow;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ProductsContext _context;
        private readonly ServiceBusTopicEventSender _serviceBusTopicEventSender;
        private readonly ServiceBusTopicEventListener _serviceBusTopicEventListener;
        private int lastEventId;
        private string lastEventStatus;

        public ProductsController(ILogger<ProductsController> logger, PayShoppingCartWorkflow payShoppingCartWorkflow, OrderCancellationWorkflow orderCancellationWorkflow, OrderModificationWorkflow orderModificationWorkflow, IHttpClientFactory httpClientFactory, ProductsContext context, ServiceBusTopicEventSender serviceBusTopicEventSender, ServiceBusTopicEventListener serviceBusTopicEventListener)
        {
            this.logger = logger;
            this.payShoppingCartWorkflow = payShoppingCartWorkflow;
            this.orderCancellationWorkflow = orderCancellationWorkflow;
            this.orderModificationWorkflow = orderModificationWorkflow;
            _httpClientFactory = httpClientFactory;
            _context = context;
            _serviceBusTopicEventSender = serviceBusTopicEventSender;
            _serviceBusTopicEventListener = serviceBusTopicEventListener;
            //_context.OrderLines.RemoveRange(_context.OrderLines);
        }

        [HttpGet("Get Products From Deposit")]
        public async Task<IActionResult> GetAllProducts([FromServices] IProductsRepository productsRepository) =>
            await productsRepository.TryGetExistingProductsDeposit().Match(
               Succ: GetAllProductsDbHandleSuccess,
               Fail: GetAllProductsDbHandleError
            );

        private ObjectResult GetAllProductsDbHandleError(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllProductsDbHandleSuccess(List<Product> products) =>
        Ok(products.Select(product => new
        {
            Code = product.Code.Value,
            Stock = product.Stock.Stock,
            Price = product.Price.Value,
        }));

        [HttpGet("Get Products From Shoppping Cart")]
        public async Task<IActionResult> GetAllProducts([FromServices] IOrdersRepository ordersRepository) =>
             await ordersRepository.TryGetExistingProducts().Match(
                Succ: GetAllProductsHandleSuccess,
                Fail: GetAllProductsHandleError
             );

        private ObjectResult GetAllProductsHandleError(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return base.StatusCode(StatusCodes.Status500InternalServerError, "UnexpectedError");
        }

        private OkObjectResult GetAllProductsHandleSuccess(List<CalculatedPrice> products) =>
        Ok(products.Select(product => new
        {
            Code = product.Code.Value,
            Quantity = product.Quantity.Value,
            Stock = product.Stock.Stock,
            Price = product.Price.Value,
            TotalPrice = product.TotalPrice
        }));

        [HttpPost("Add Product To Shopping Cart")]
        public async Task<IActionResult> PublishProducts([FromBody] InputProduct[] products)
        {
            var unvalidatedProducts = products.Select(MapInputProductToUnvalidatedProducts)
                                          .ToList()
                                          .AsReadOnly();
            ProcessOrderCommand command = new(unvalidatedProducts);
            var result = await payShoppingCartWorkflow.ExecuteAsync(command);

            return await result.MatchAsync(
                whenOrderProcessingFailedEvent: HandleFailure,
                whenOrderProcessingSucceededEvent: HandleSuccess
            );
        }

        [HttpPost("Add Client Info")]
        public async Task<IActionResult> ClientInfoAsync([FromServices] IOrdersRepository ordersRepository, [FromBody] InputClient clientModel)
        {
            var finalPrice = ordersRepository.CalculateFinalPrice();
            if (ModelState.IsValid)
            {
                // Adaugă clientul în baza de date
                var order = new OrderHeaderDto(clientModel.Name, clientModel.Address, finalPrice);
                //_context.OrderHeaders.Add(new(clientModel.Name, clientModel.Address, finalPrice));
                _context.OrderHeaders.Add(order);
                //_context.OrderLines.RemoveRange(_context.OrderLines); //ca sa stearga produsele din cos cand se introduc datele clientului
                _context.SaveChanges();

                Console.WriteLine("ORDER ID IS: " + order.OrderId);

                // Exemplu în care comanda este preluată
                var orderPlacedEvent = new OrderPublishedEvent
                {
                    Id = order.OrderId,
                    Status = "placed"
                };

                var result = await _serviceBusTopicEventSender.SendAsync("topic1", orderPlacedEvent);

                if (result.IsSuccess)
                {
                    return Ok("Client added successfully!");
                    // Successfully sent
                }
                else
                {
                    return BadRequest("Invalid data for client!");
                    // Handle the failure
                }

            }

            return BadRequest("Invalid data for client!");
        }

        private ManualResetEvent processEventCompleted = new ManualResetEvent(false);

        [HttpPost("Place Order")]
        public async Task<IActionResult> PlaceOrder([FromServices] IOrdersRepository ordersRepository, [FromServices] SharedData sharedData, CancellationToken cancellationToken = default)
        {
            await _serviceBusTopicEventListener.StartAsync("topic1", "s1", cancellationToken);
            _serviceBusTopicEventListener.ProcessMessageAsCloudEvent += ProcessCloudEvent;
            processEventCompleted.WaitOne();
            int id = lastEventId;
            var orderInfo = await ordersRepository.TryGetClientOrderInfo(id);
            var productsResult = await ordersRepository.TryGetExistingProducts();
            List<CalculatedPrice> products = productsResult.Match(
                Succ: products => products,
                Fail: ex =>
                {
                    // Tratați cazul în care obținerea produselor a eșuat
                    // Exemplu: Afișați un mesaj de eroare sau tratați în alt mod

                    // Întoarceți o listă goală sau altă valoare predeterminată, în funcție de nevoi
                    return new List<CalculatedPrice>();
                }
            );
            var projectedProducts = products.Select(p => new
            {
                Code = p.Code.Value,
                Quantity = p.Quantity.Value,
                Price = p.Price.Value,
                TotalPrice = p.TotalPrice,
                // Adăugați orice alte proprietăți necesare
            }).ToList();
            OrderInvoice orderInvoice = null;
            orderInfo.Match(
            Succ: orderOk =>
            {
                orderInvoice = new OrderInvoice
                {
                    // Acum puteți utiliza obiectul orderInfo
                    Message = "ORDER INVOICE",
                    ClientName = orderOk.Client.Name,
                    ClientAddress = orderOk.Client.Address,
                    Products = projectedProducts.Cast<object>().ToList(),
                    FinalPrice = orderOk.FinalPrice,
                    Message2 = "The delivery process has been initiated!"
                };
                //ordersRepository.TryUpdateProductStock();
                int cancelOrderYesOrNo = 0;
                ordersRepository.UpdateProductStock(cancelOrderYesOrNo);
            },
            Fail: ex =>
            {
                orderInvoice = new OrderInvoice
                {
                    Message = "Failed to retrieve order information"
                    // Puteți seta alte proprietăți ale orderInvoice aici, în funcție de necesități
                };
            });
            var orderProcessedEvent = new OrderPublishedEvent
            {
                Id = id,
                Status = "processed"
            };

            //await _serviceBusTopicEventListener.StopAsync(cancellationToken);
            var result = await _serviceBusTopicEventSender.SendAsync("topic1", orderProcessedEvent);
            sharedData.OrderId = lastEventId;
            sharedData.OrderStatus = lastEventStatus;

            if (result.IsSuccess)
            {
                Console.WriteLine("ORDER PROCESSED STATUS SENT!");
                // Successfully sent
            }
            else
            {
                Console.WriteLine("ORDER PROCESSED STATUS NOT SENT!");
                // Handle the failure
            }
            return Ok(orderInvoice);
        }

        [HttpPost("Cancel Order")]
        public async Task<IActionResult> CancelOrder([FromServices] IOrdersRepository ordersRepository, [FromServices] SharedData sharedData, CancellationToken cancellationToken = default)
        {
            //await _serviceBusTopicEventListener.StartAsync("topic1", "s1", cancellationToken);
            //_serviceBusTopicEventListener.ProcessMessageAsCloudEvent += ProcessCloudEvent;
            int orderId = sharedData.OrderId;
            //string orderStatus = sharedData.OrderStatus;
            string orderStatus = "processed";
            OrderCancellationCommand command = new(orderId, orderStatus);
            var result = await orderCancellationWorkflow.CancelOrderAsync(command);

            return await result.MatchAsync(
                whenOrderCancellationFailedEvent: HandleOrderCancellationFailure,
                whenOrderCancellationSucceededEvent: HandleOrderCancellationSuccess
            );
        }

        private Task<IActionResult> HandleOrderCancellationFailure(OrderCancellationEvent.OrderCancellationFailedEvent failedEvent)
        {
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason));
        }

        private Task<IActionResult> HandleOrderCancellationSuccess(OrderCancellationEvent.OrderCancellationSucceededEvent successEvent)
        {
            return Task.FromResult<IActionResult>(Ok("The order has been cancelled!"));
        }



        [HttpPost("Modify Order")]
        public async Task<IActionResult> ModifyOrder([FromServices] IOrdersRepository ordersRepository, CancellationToken cancellationToken = default)
        {
            await _serviceBusTopicEventListener.StartAsync("topic1", "s1", cancellationToken);
            _serviceBusTopicEventListener.ProcessMessageAsCloudEvent += ProcessCloudEvent;
            return Ok("The order has been modified!");
        }


        private async Task ProcessCloudEvent(CloudNative.CloudEvents.CloudEvent cloudEvent)
        {
            // Logica de procesare pentru evenimente
            var eventData = JsonConvert.DeserializeObject<OrderPublishedEvent>(cloudEvent.Data.ToString());
            lastEventId = eventData.Id;
            lastEventStatus = eventData.Status;
            Console.WriteLine($"Order status received: {cloudEvent.Type}, Status: {cloudEvent.Data}, Id: {eventData.Id}");
            //Console.WriteLine($"Eveniment id: {eventData.Id}");
            processEventCompleted.Set();
            // Aici puteți adăuga orice logică suplimentară pentru a trata evenimentul
        }

        private Task<IActionResult> HandleFailure(OrderProcessingEvent.OrderProcessingFailedEvent failedEvent)
        {
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason));
        }

        private Task<IActionResult> HandleSuccess(OrderProcessingEvent.OrderProcessingSucceededEvent successEvent)
        {
            // var w1 = TriggerReportGeneration(successEvent);
            //var w2 = TriggerScholarshipCalculation(successEvent);
            //await Task.WhenAll(w1, w2);
            //return Ok();
            return Task.FromResult<IActionResult>(Ok("The product has been added successfully!"));
        }

        private async Task<Boolean> TriggerReportGeneration(OrderProcessingEvent.OrderProcessingSucceededEvent successEvent)
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7114/Home/GenReport")
            {
                Content = new StringContent(JsonConvert.SerializeObject(successEvent), Encoding.UTF8, "application/json")
            };
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(httpRequestMessage);

            // Verifică dacă cererea a fost reușită
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Report page opened successfully.");
                return true;
            }

            // Tratează cazul în care cererea a eșuat
            Console.WriteLine("Request failed with status code: " + response.StatusCode);
            return false;

        }

        private async Task<Boolean> TriggerScholarshipCalculation(OrderProcessingEvent.OrderProcessingSucceededEvent successEvent)
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7072/report/scholarship")
            {
                Content = new StringContent(JsonConvert.SerializeObject(successEvent), Encoding.UTF8, "application/json")
            };
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(httpRequestMessage);
            return true;
        }

        private static UnvalidatedProduct MapInputProductToUnvalidatedProducts(InputProduct product) => new UnvalidatedProduct(
            Code: product.Code,
            Quantity: product.Quantity,
            Stock: 3,
            Price: 6);

        private static Exemple.Domain.Models.Client MapInputProductToClient(InputClient client) => new Exemple.Domain.Models.Client(client.Name, client.Address);
    }
}

