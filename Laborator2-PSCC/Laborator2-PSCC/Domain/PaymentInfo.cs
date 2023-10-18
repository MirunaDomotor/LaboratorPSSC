﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator2_PSCC.Domain
{
    public record PaymentInfo
    {
        public string PaymentMethod { get; set; }
        public PaymentInfo(string method)
        {
            PaymentMethod = method;
        }
    }
}
