using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using CEMVC.Core.DAL;
using CEMVC.FrontEnd.Web.Core.Extentions;
using CEMVC.FrontEnd.Web.Models.Special;
using System;

namespace CEMVC.FrontEnd.Web.Models.Auth
{
    public class UpgradeInvoiceSummaryModel
    {
        [Display(Name = "Protected Credit from Current Plan:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ProtectedCredit { get; set; }

        [Display(Name = "Protected Charge for New Plan:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal ProtectedCharge { get; set; }

        [Display(Name = "Total:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Total { get; set; }

        [Display(Name = "Credits:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Credits { get; set; }

        [Display(Name = "Amount Due Today:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal AmountDueToday { get; set; }

        [Display(Name = "Next bill:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal NextBill { get; set; }

        [Display(Name = "on")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime NextBillDate { get; set; }

    }
}