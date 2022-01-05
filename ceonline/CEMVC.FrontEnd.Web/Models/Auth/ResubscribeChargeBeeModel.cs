using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models.Auth
{
    public class ResubscribeChargeBeeModel
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address:")]
        public string Email { get; set; }

        [Display(Name = "Company:")]
        public string Company { get; set; }

        public string RegistrationToken { get; set; }
        public string url { get; set; }
        public string hosted_page_id { get; set; }
        public string site_name { get; set; }
        public string message { get; set; }
        public string errorMessage { get; set; }

    }
}