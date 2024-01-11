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
    public class ProductsPaidEventHandler : AbstractEventHandler<OrderPublishedEvent>
    {
        public override string[] EventTypes => new string[] { typeof(OrderPublishedEvent).Name };


        protected override Task<EventProcessingResult> OnHandleAsync(OrderPublishedEvent eventData)
        {
            Console.WriteLine(eventData.ToString());
            return Task.FromResult(EventProcessingResult.Completed);
        }
    }
}
