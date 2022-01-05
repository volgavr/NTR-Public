using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CEMVC.Core.BLL.Infrastructure;
using Kendo.Mvc.Extensions;

namespace CEMVC.FrontEnd.Web.Models.Project
{
    public class TimeCardViewModel
    {
        public int ProjectId { get; private set; }

        public IEnumerable<TimeCardTotalLineViewModel> LineItems { get; set; }

        public TimeCardViewModel(int projectId, IEnumerable<TimeCardCategorySummarized> items)
        {
            ProjectId = projectId;
            LineItems = items.Select(i => new TimeCardTotalLineViewModel { CategoryId = i.Id, CategoryTitle = i.Title, Hours = i.Hours });
        }
    }

    public class TimeCardTotalLineViewModel
    {
        public int CategoryId { get; set; }
        [Display(Name = "Category")]
        public string CategoryTitle { get; set; }
        [Display(Name = "Total Hours")]
        public decimal Hours { get; set; }
    }
}