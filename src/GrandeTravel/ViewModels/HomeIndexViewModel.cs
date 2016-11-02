using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GrandeTravel.Models;

namespace GrandeTravel.ViewModels
{
    public class HomeIndexViewModel
    {
        public string Destination { get; set; }
        public IEnumerable<PackageWithRating> BestPackagesWithRatings { get; set; }

    }
}
