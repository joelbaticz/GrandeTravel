using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandeTravel.Models;

namespace GrandeTravel.ViewModels
{
    public class PayPackageViewModel
    {
        public int BookingId { get; set; }
        public string PackageName { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateFor { get; set; }
        public int NumberOfPeople { get; set; }
        public double TotalPrice { get; set; }

        /*public PaymentOptionsEnum PaymentType { get; set; }*/
        public int PaymentType { get; set; }
    }
}
