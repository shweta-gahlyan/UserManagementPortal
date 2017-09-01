using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UserMgmtMVC.Models
{
    public class AppUserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "User name")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Password is required", AllowEmptyStrings = false)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 7)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }

        public string RoleName { get; set; }
    }
}