using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models.Auth
{
    public class RegisterChargeBeeModel
    {
        [Required(ErrorMessage = "Email address is Required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address:")]
        public string Email { get; set; }

        public string RegistrationToken { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        [StringLength(50, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password:")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public double? Longitude { get; set; }

        public double? Latitude { get; set; }

    }
}