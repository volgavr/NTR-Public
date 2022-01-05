using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email:")]
        [EmailAddress(ErrorMessage = "Email Address is Invalid")]
        public string UserEmail { get; set; }
    }
}