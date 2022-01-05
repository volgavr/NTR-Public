using System;
using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models.Auth
{
    public class LoginModel
    {
//        [Required]
//        [Display(Name = "Login:")]
//        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Email Address is Invalid")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Display(Name = "Error message:")]
        [DataType(DataType.Html)]
        public string ErrorMessage { get; set; }

        public string LoginTime { get; set; }

        public int ClientTimeOffset { get; set; }

        public bool ReCaptchaRequired { get; set; }
    }
}