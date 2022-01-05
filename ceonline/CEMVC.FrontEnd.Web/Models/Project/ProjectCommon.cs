using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEMVC.FrontEnd.Web.Models.Project
{
    public class ProjectCommon
    {        
        public int Id { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }

        public string Number { get; set; }
        
        
        
        public DateTime? Date { get; set; }
        
        public int? Status { get; set; }

        public bool IncludeMarkup { get; set; }

        public static ProjectCommon FromBO(CEMVC.Core.DAL.Project project)
        {
            return new ProjectCommon
            {
                Id = project.id,
                Title = project.title,
                Number = project.is_component == true ? project.title : "",
                Description = project.description,
                Date = project.project_date,
                
                Status = project.status_id,
                IncludeMarkup = project.include_markup.HasValue && project.include_markup.Value
            };
        }
    }
}