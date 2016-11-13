using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandeTravel.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int PackageId { get; set; }
        public DateTime DateMade { get; set; }
        public string CompanyName { get; set; }
        public string PackageName { get; set; }
        public DateTime DateFor { get; set; }
        public int NumberOfPeople { get; set; }
        public string SpecialRequirements { get; set; }
        public double Price { get; set; }
        public bool IsPaid { get; set; }

        //relationship
        public int CustomerId { get; set; }



        //association
        public Customer Customer { get; set; }


    }
}
