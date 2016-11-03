using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandeTravel.Models
{
    public class Customer
    {

        public int CustomerId { get; set; }

        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //Relationships
        public List<Booking> Booking { get; set; }

    
    }
}
