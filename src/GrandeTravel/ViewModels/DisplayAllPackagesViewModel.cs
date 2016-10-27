using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GrandeTravel.Models;

namespace GrandeTravel.ViewModels
{
    public class DisplayAllPackagesViewModel
    {
        public string Location { get; set; }
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }

        public IEnumerable<Package> Packages { get; set; }
    }
}
