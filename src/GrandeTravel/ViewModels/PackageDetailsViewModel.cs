using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GrandeTravel.Models;

namespace GrandeTravel.ViewModels
{
    public class PackageDetailsViewModel
    {
        public int ProviderId { get; set; }
        public int PackageId { get; set; }
        public string Name { get; set; }

        public double Rating { get; set; }
        public int NumberOfReviews { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public double Price { get; set; }

        public IEnumerable<Feedback> Feedbacks { get; set; }
    }
}
