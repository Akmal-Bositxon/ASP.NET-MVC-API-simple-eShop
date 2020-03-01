using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WAD.CW2_WebApp._5529.Models
{
    public class LoginViewModel
    {
        [DisplayName("User Name / Login")]
        [Required]
        public string UserName { get; set; }
        [DisplayName("Password")]
        [Required]
        public string Password { get; set; }
    }
}