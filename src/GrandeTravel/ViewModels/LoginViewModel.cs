using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace GrandeTravel.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }



        //A browser functionality for RememberMe
        public bool RememberMe { get; set; }


        public string ReturnUrl { get; set; }
    }
}
