using System;
using System.ComponentModel.DataAnnotations;
using Exemple.Domain.Models;

namespace Example.API.Models
{
    public class InputProduct
    {
        [Required]
        [RegularExpression(ProductCode.Pattern)]
        public string Code { get; set; }

        [Required]
        public int Quantity { get; set; }

        //[Required]
        //public int Stoc { get; set; }

        //[Required]
        //public double Price { get; set; }
    }
}
