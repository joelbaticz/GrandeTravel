using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandeTravel.Models
{
    public class Package
    {

        public int PackageId { get; set; }
        public string Name { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsActive { get; set; }

        //relationship
        public int ProviderId { get; set; }

        public List<Feedback> Feedbacks { get; set; }


        //association
        public Provider Provider { get; set; }


    }
}
