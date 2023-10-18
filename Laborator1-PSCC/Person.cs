using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laborator1PSCC
{
    internal record Person
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Person(string lastName, string firstName, string phoneNumber, string address)
        {
            LastName = lastName;
            FirstName = firstName;
            PhoneNumber = phoneNumber;
            Address = address;
        }
        public void Write()
        {
            Console.WriteLine("Last name=" + LastName + " " + "First name=" + FirstName + " " + "Phone number=" + PhoneNumber + " " + "Address=" + Address + "\n");
        }
    }
}