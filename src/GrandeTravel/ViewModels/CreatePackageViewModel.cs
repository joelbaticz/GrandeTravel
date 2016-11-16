using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace GrandeTravel.ViewModels
{
    public class CreatePackageViewModel
    {
        [Required]
        public int ProviderId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Location { get; set; }
        [Required]
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public IEnumerable<string> ImageNames { get; set; }


    }
}
