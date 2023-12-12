using Example.Events.Models;
using Example.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.Dto;

namespace Example.Accomodation.EventProcessor
{
    internal class ProductsPaidEventHandler : AbstractEventHandler<ProductsPublishedEvent>
    {
        public override string[] EventTypes => new string[] { typeof(ProductsPublishedEvent).Name };

        protected override Task<EventProcessingResult> OnHandleAsync(ProductsPublishedEvent eventData)
        {
            Console.WriteLine(eventData.ToString());
            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
}
