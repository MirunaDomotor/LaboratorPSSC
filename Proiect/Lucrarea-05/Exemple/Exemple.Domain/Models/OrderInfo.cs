using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple.Domain.Models
{
    public class OrderInfo
    {
        public Client Client { get; set; }
        public double FinalPrice { get; set; }
        public OrderInfo(Client Client, double FinalPrice)
        {
            this.Client = Client;
            this.FinalPrice = FinalPrice;
        }
        public OrderInfo()
        {
        }
    }
}
