using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static Laborator1PSCC.Quantity;
using System.Globalization;
using System.Net;

namespace Laborator1PSCC
{
    class Program
    {
        private static void Main(string[] args)
        {
            var ok = "";
            List<Product> listOfProducts = new();
            Person contact = ReadContact();
            do
            {
                Product product = ReadProduct();
                listOfProducts.Add(product);
                ok = ReadValue("Add more products? If so, press '1'! ");
            } while (ok == "1");
            Order order = new Order(contact, listOfProducts);
            order.Write();
        }

        private static IQuantity QuantityConversion(string quantity)
        {
            if (Int32.TryParse(quantity, out int units))
                return new Unit(units);
            else if (Double.TryParse(quantity, NumberStyles.Number, CultureInfo.CreateSpecificCulture("en-US"), out double kgs))
                return new Kg(kgs);
            else
                return new Undefined(quantity);
        }

        private static Person ReadContact()
        {
            Console.WriteLine("Enter contact");
            var lastName = ReadValue("Last name: ");
            while (String.IsNullOrEmpty(lastName))
            {
                Console.WriteLine("Last name cannot be empty!");
                lastName = ReadValue("Please enter last name: ");
            }
            var firstName = ReadValue("First name: ");
            while (String.IsNullOrEmpty(firstName))
            {
                Console.WriteLine("Last name cannot be empty!");
                firstName = ReadValue("Please enter first name: ");
            }
            var phoneNumber = ReadValue("Phone number: ");
            while (String.IsNullOrEmpty(phoneNumber))
            {
                Console.WriteLine("Phone number cannot be empty!");
                phoneNumber = ReadValue("Please enter phone numer: ");
            }
            var address = ReadValue("Address: ");
            while (String.IsNullOrEmpty(address))
            {
                Console.WriteLine("Address cannot be empty!");
                address = ReadValue("Please enter address: ");
            }
            Person contact = new Person(lastName, firstName, phoneNumber, address);
            return contact;
        }

        public static Product ReadProduct()
        {
            Console.WriteLine("Enter product");
            var code = ReadValue("Code: ");
            while (String.IsNullOrEmpty(code))
            {
                Console.WriteLine("Code cannot be empty!");
                code = ReadValue("Please enter code: ");
            }
            IQuantity quantity = QuantityConversion(ReadValue("Quantity: "));
            Product product = new Product(code, quantity);
            return product;
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}