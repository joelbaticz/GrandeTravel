using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandeTravel.ViewModels
{
    public class BookPackageViewModel
    {
        public int PackageId { get; set; }
        public int CustomerId { get; set; }
        public int ProviderId { get; set; }
        public string PackageName { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateFor { get; set; }
        public int NumberOfPeople { get; set; }
        public string SpecialRequirements { get; set; }
        public double Price { get; set; }

    }
}
