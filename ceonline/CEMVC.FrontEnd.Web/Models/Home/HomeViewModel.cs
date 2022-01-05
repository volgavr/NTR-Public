using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models.Home
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            RecentProjects = new List<HomePageProject>();
            Customers = new List<HomePageProject>();
        }
        public List<HomePageProject> RecentProjects { get; set; }

        public List<HomePageProject> Customers { get; set; }

        [Display(Name = "Show Archived Customers & Projects")]
        public bool showArchived { get; set; }

    }
}