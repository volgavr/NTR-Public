using CEMVC.Core.BLL.Enums;
using CEMVC.Core.DAL;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models.Common
{
    public class SendInvoiceDialogModel
    {
        #region Constructor
        public SendInvoiceDialogModel()
        {
        }
        public SendInvoiceDialogModel(int invoiceId, Customer client, string emailFrom, string subject, string body, int projectId, string paymentUrl = "")
        {
            InvoiceId = invoiceId;
            ProjectId = projectId;
            this.From = emailFrom;            
            this.To = client.email_business;
            this.SendMeCopy = false;

            var subj = "invoice";
            Subject = (subject ?? "").Replace("{{Subject}}", subj);
            Body = (body ?? "").Replace("{{Subject}}", subj).Replace("{{PayUrl}}", paymentUrl);

            this.toList = new List<DropDownListItem>();
            if (!string.IsNullOrWhiteSpace(client.email_business))
            {
                this.To = client.email_business;
                toList.Add(new DropDownListItem { Text = client.email_business, Value = client.email_business, Selected = true });
            }
            if (!string.IsNullOrWhiteSpace(client.email_personal))
            {
                if (toList.Count == 0)
                    this.To = client.email_personal;
                toList.Add(new DropDownListItem { Text = client.email_personal, Value = client.email_personal, Selected = false });
            }
        }
        #endregion

        #region Properties

        [Display(Name = "From:")]
        public string From { get; set; }

        [Display(Name = "To:")]
        public string To { get; set; }

        [Display(Name = "Subject:")]
        public string Subject { get; set; }

        [Display(Name = "Body:")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; }

        [Display(Name = "Send me a copy")]
        public bool SendMeCopy { get; set; }

        public int InvoiceId { get; set; }

        public int ProjectId { get; set; }


        public List<DropDownListItem> toList { get; set; }

        #endregion

        #region Convertors
        #endregion
        public void Preprocess(Customer customer, Company_Info userCompany, string link)
        {
            if (this.Subject != null)
                this.Subject = this.Subject
                    .Replace("{{Customer Name}}", customer != null ? customer.first_name : "")
                    .Replace("{{Company Name}}", userCompany != null ? userCompany.company_name : "");

            if (this.Body != null)
                this.Body = this.Body
                    .Replace("{{Customer Name}}", customer != null ? customer.first_name : "")
                    .Replace("{{Link}}", link)
                    .Replace("{{Company Name}}", userCompany != null ? userCompany.company_name : "");
        }
    }


}