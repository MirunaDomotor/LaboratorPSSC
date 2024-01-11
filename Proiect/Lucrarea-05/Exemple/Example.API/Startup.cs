using Azure.Messaging.ServiceBus;
using Example.Accomodation.EventProcessor;
using Example.API.Models;
using Example.Data;
using Example.Data.Repositories;
using Example.Events;
using Example.Events.ServiceBus;
using Exemple.Domain;
using Exemple.Domain.Repositories;
using Exemple.Domain.Workflows;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Example.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProductsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IOrdersRepository, OrdersRepository>();
            services.AddTransient<IProductsRepository, ProductsRepository>();
            services.AddTransient<PayShoppingCartWorkflow>();
            services.AddTransient<OrderCancellationWorkflow>();
            services.AddTransient<OrderModificationWorkflow>();

            //services.AddSingleton<ServiceBusClient>();
            string serviceBusConnectionString = Configuration.GetConnectionString("ServiceBus");
            services.AddSingleton(x => new ServiceBusClient(serviceBusConnectionString));

            services.AddSingleton<IEventHandler, ProductsPaidEventHandler>();
            services.AddSingleton<ServiceBusTopicEventSender>();
            services.AddSingleton<ServiceBusTopicEventListener>();
            services.AddSingleton<SharedData>();

            services.AddHttpClient();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Example.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
