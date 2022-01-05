using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models
{
    public class RecoveryPasswordModel
    {
        [Required(ErrorMessage = "Secure Code is Required")]
        [Display(Name = "Secure Code:")]
        public string SecureCode { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password:")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "New Password must be confirmed")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage="Don't match with password")]
        [Display(Name = "Confirm New Password:")]
        public string ConfirmPassword { get; set; }
    }
}