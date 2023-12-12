﻿using Exemple.Domain;
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

namespace Example.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private ILogger<ProductsController> logger;
        private readonly PayShoppingCartWorkflow payShoppingCartWorkflow;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ProductsContext _context;

        public ProductsController(ILogger<ProductsController> logger, PayShoppingCartWorkflow payShoppingCartWorkflow, IHttpClientFactory httpClientFactory, ProductsContext context)
        {
            this.logger = logger;
            this.payShoppingCartWorkflow = payShoppingCartWorkflow;
            _httpClientFactory = httpClientFactory;
            _context = context;
        }

        [HttpGet("getProductsFromDeposit")]
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

        [HttpGet("getProductsFromShopppingCart")]
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

        [HttpPost("addProductToShoppingCart")]
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

        [HttpPost("addClientInfo")]
        public IActionResult ClientInfo([FromServices] IOrdersRepository ordersRepository, [FromBody] InputClient clientModel)
        {
            var finalPrice = ordersRepository.CalculateFinalPrice();
            if (ModelState.IsValid)
            {
                // Adaugă clientul în baza de date
                _context.OrderHeaders.Add(new(clientModel.Name, clientModel.Address, finalPrice));
                _context.OrderLines.RemoveRange(_context.OrderLines);
                _context.SaveChanges();

                return Ok("Client added successfully!");
            }

            return BadRequest("Invalid data for client!"); 
        }

        private Task<IActionResult> HandleFailure(OrderProcessingEvent.OrderProcessingFailedEvent failedEvent)
        {
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError, failedEvent.Reason));
        }

        private async Task<IActionResult> HandleSuccess(OrderProcessingEvent.OrderProcessingSucceededEvent successEvent)
        {
            var w1 = TriggerReportGeneration(successEvent);
            var w2 = TriggerScholarshipCalculation(successEvent);
            await Task.WhenAll(w1, w2);
            return Ok();
        }

        private async Task<Boolean> TriggerReportGeneration(OrderProcessingEvent.OrderProcessingSucceededEvent successEvent)
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7286/report/semester-report")
            {
                Content = new StringContent(JsonConvert.SerializeObject(successEvent), Encoding.UTF8, "application/json")
            };
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(httpRequestMessage);
            return true;
        }

        private async Task<Boolean> TriggerScholarshipCalculation(OrderProcessingEvent.OrderProcessingSucceededEvent successEvent)
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            "https://localhost:7286/report/scholarship")
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

        private static Client MapInputProductToClient(InputClient client) => new Client(
            Name: client.Name,
            Address: client.Address);
    }
}