using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CEMVC.Core.DAL;
using CEMVC.FrontEnd.Web.Core.Utils;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace CEMVC.FrontEnd.Web.Models.Common
{
    public class PaymentScheduleEditorModel
    {
        #region Constructor
        public PaymentScheduleEditorModel()
        {
        }
        #endregion

        #region Properties
        [Display(Name = "Payment Schedule Id")]
        public int ScheduleId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Default for Projects")]
        public bool? IsDefault { get; set; }

        public IEnumerable<PaymentScheduleItem> Items { get; set; }

        public static PaymentScheduleEditorModel FromBO(Schedule src)
        {
            if (src == null)
                throw new ArgumentNullException();

            var ps = ServicesFactory.Instance.GetScheduleService().GetPaymentScheduleContent(src);

            return new PaymentScheduleEditorModel
            {
                ScheduleId = ps.id,
                Title = ps.title,
                Description = ps.description,
                Items = ps.items?.Select(x => new PaymentScheduleItem()
                {
                    type = x.type,
                    description = x.description,
                    formalText = x.formalText,
                    preliminaryText = x.preliminaryText,
                    invoiceText = x.invoiceText,
                    value = x.value
                })
            };
        }

        public static Schedule ToBO(PaymentScheduleEditorModel src)
        {
            return new Schedule
            {
                title = src.Title?.Trim(),
                description = src.Description
            };
        }

        public void UpdateBO(Schedule target)
        {
            if (target.id <= 0)
                throw new InvalidOperationException("Object to be updated has ID = 0");

            if (ScheduleId == target.id)
            {
                target.title = Title?.Trim();
                target.description = Description;
            }
            else
                throw new InvalidOperationException("Update Violation: Source object ID does not match ID of the object to update");
        }
        #endregion
    }

    [Serializable]
    public class PaymentScheduleItem
    {
        [NonSerialized]
        public readonly string id;

        public int? type { get; set; }

        public decimal? value { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Preliminary Text")]
        public string preliminaryText { get; set; }

        [Display(Name = "Formal Text")]
        public string formalText { get; set; }

        [Display(Name = "Invoice Text")]
        public string invoiceText { get; set; }

        public static List<PaymentScheduleItem> ProcessScheduleItemsMoney(IEnumerable<PaymentScheduleItem> items, decimal total)
        {
            var list = items.Where(x => x.type != 3).ToList();
            var result = new List<PaymentScheduleItem>();
            foreach (var x in list)
            {
                if (x.type == 2)
                    x.value = total * x.value;
                result.Add(x);
            }

            var due = items.FirstOrDefault(x => x.type == 3);
            if (due != null)
            {
                var cost = Math.Round(result.Sum(x => x.value).GetValueOrDefault(), 2, MidpointRounding.AwayFromZero);
                due.value = total - cost;
                result.Add(due);
            }
            return result;
        }

        public static SelectListItem ConvertToListItem(PaymentScheduleItem o)
        {
            return new SelectListItem()
            {
                Value = JsonConvert.SerializeObject(new
                {
                    Amount = o.value ?? 0,
                    InvoiceTitle = (string.IsNullOrEmpty(o.invoiceText) ? o.description : o.invoiceText).Replace("[val]", (o.value.HasValue) ? o.value.Value.ToString("C2") : "[val]")
                }),
                Text = o.description // (o.description.Length > 50) ? o.description.Substring(0, 50) + "..." : o.description
            };
        }

    }

    [Serializable]
    public class PaymentScheduleContents
    {
        public string description { get; set; }

        public PaymentScheduleItem[] items { get; set; }
    }
}