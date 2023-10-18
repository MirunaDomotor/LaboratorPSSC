using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator1PSCC
{
    internal record Order
    {

        public Person Contact { get; set; }
        public List<Product> ProductsList { get; set; }
        public Order(Person person, List<Product> products)
        {
            Contact = person;
            ProductsList = products;
        }
        public void Write()
        {
            Contact.Write();
            foreach(var product in ProductsList){
                product.Write();
            }
            Console.WriteLine();
        }
    }
}