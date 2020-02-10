using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheFortress.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserNameReg { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@".*[^0-9a-zA-Z].*", ErrorMessage = "The password must include at least one non-alphanumeric character.")]
        [Display(Name = "Password")]
        public string PasswordReg { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("PasswordReg", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
        [Display(Name = "Check here for Artist account")]
        public bool IsArtist { get; set; }
    }
}
