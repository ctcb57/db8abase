using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace db8abase.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        [Display(Name = "Street Addres")]
        public string StreetAddress { get; set; }
        public string City { get; set; }
        [Display(Name = "State Abbreviation" )]
        public string StateAbbreviation { get; set; }
        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
