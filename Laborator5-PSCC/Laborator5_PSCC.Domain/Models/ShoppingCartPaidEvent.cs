using System;
using CSharp.Choices;

namespace Laborator5_PSSC.Domain.Models
{
	[AsChoice]
	public static partial class OrderProcessingEvent
	{
		public interface IOrderProcessingEvent { }

		public record OrderProcessingSucceededEvent : IOrderProcessingEvent
		{
			public string Csv { get; }
			public DateTime PaidDate { get; }

			internal OrderProcessingSucceededEvent(string csv, DateTime paidDate)
			{
				Csv = csv;
				PaidDate = paidDate;
			}
		}

        public record OrderProcessingFailedEvent : IOrderProcessingEvent
        {
            public string Reason { get; }

            internal OrderProcessingFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}

