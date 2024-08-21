using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Domain.Shipping
{
    public class ShippingDetails
    {
        string address;
        string name;
        string city;
        string country;
        string zipcode;

        public ShippingDetails(string name, string city, string address, string country, string zipcode)
        {
            this.name = name;
            this.city = city;
            this.address = address;
            this.country = country;
            this.zipcode = zipcode;
        }
        

        public String Name {get => name; set => name = value; }
        public String City {get => city; set => city = value; }
        public String Address {get => address; set => address = value; }
        public String Country {get => country; set => country = value; }
        public String Zipcode {get => zipcode; set => zipcode = value; }
    

        // public override string ToString()
        // {
        //     return $"Name: {Name}, City: {City}, Address: {Address}, Country: {Country}, Zipcode: {Zipcode}";
        // }
    }
}