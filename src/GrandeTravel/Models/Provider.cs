using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandeTravel.Models
{
    public class Provider
    {

        public int ProviderId { get; set; }
        public string UserId { get; set; }

        //public int Id { get; set; }

        public string DisplayName { get; set; }
        //public string Email { get; set; }
        //public string Phone { get; set; }
        public string ABN { get; set; }

        //public string CompanyUrl { get; set; }
        //public string Address { get; set; }


        //Relationships
        public List<Package> Packages { get; set; }

        //public IdentityUser User { get; set; }

    }
}
