using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CEMVC.FrontEnd.Web.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Current Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password:")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New Password is required")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password:")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm New Password is required")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage= "Confirm Password doesn't match New Password.")]
        [Display(Name = "Confirm new password:")]
        public string ConfirmPassword { get; set; }
    }
}