using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WAD.CW2_WebApp._5529.Models
{
    public class RegistrationViewModel
    {
        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }
        [DisplayName("User Name / Login")]
        [Required]
        public string UserName { get; set; }
        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }
        [DisplayName("Comfirm Password")]
        [Required]
        public string ComfirmPassword { get; set; }
        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}