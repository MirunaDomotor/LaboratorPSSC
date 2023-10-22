using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record PaymentInfo
    {
        private static readonly Regex ValidPattern1 = new("^card$");
        private static readonly Regex ValidPattern2 = new("^cash$");
        public string PaymentMethod { get; set; }
        public decimal PayAmount { get; set; }
        public PaymentInfo(string method, decimal payAmount)
        {
            if (IsValid(method))
            {
                PaymentMethod = method;
                PayAmount = payAmount;
            }
        }
        private static bool IsValid(string stringValue) => ValidPattern1.IsMatch(stringValue) || ValidPattern2.IsMatch(stringValue);
    }
}
