using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models.Auth
{
    public class ResubscribeModel
    {
        [Required(ErrorMessage = "Email address is Required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Activation Key is Required")]
        [Display(Name = "Activation Key:")]
        public string ActKey { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}