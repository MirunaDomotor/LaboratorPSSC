
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Dto
{
    [JsonObject(MemberSerialization.OptIn)]
    public record OrderPublishedEvent
    {
        //public List<ProductCartDto> Products { get; init; }
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }
        //[JsonProperty("ClientName")]
        //public string ClientName { get; set; }
        //[JsonProperty("ClientAddress")]
        //public string ClientAddress { get; set; }
        //[JsonProperty("TotalPrice")]
        //public double TotalPrice { get; set; }
    }
}
