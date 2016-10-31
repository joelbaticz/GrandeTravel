using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace GrandeTravel.ViewModels
{
    public class LeaveFeedbackViewModel
    {
        [Required]
        public int PackageId { get; set; }
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string PackageName { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
