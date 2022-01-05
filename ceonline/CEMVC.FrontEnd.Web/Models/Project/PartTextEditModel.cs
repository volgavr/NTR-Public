using CEMVC.Core.DAL;
using System.ComponentModel.DataAnnotations;

namespace CEMVC.FrontEnd.Web.Models.Project
{
    public class PartTextEditModel
    {
        public int Id { get; set; }
        [StringLength(255, ErrorMessage = "The part description length should be less then 255 characters.")]
        public string Title { get; set; }

        public string PreliminaryText { get; set; }
        public string FormalText { get; set; }
        public string SubcontractorText { get; set; }

        public static PartTextEditModel FromBO(Project_Parts part)
        {
            return new PartTextEditModel {
                Id = part.id,
                Title = part.part_title,
                PreliminaryText = part.preliminary,
                FormalText = part.formal,
                SubcontractorText = part.subcontractor
            };
        }

        public void UpdateBO(Project_Parts part)
        {
            if (!string.IsNullOrEmpty(Title))
                part.part_title = Title;
            part.preliminary = PreliminaryText;
            part.formal = FormalText;
            part.subcontractor = SubcontractorText;
        }
    }
}