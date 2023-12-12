using System;
using System.ComponentModel.DataAnnotations;
using Exemple.Domain.Models;

namespace Example.API.Models
{
    public class InputClient
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
