using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CEMVC.FrontEnd.Web.Models.Project
{
    //public class ProjectPartComparer : IComparer<ProjectPartListItem>
    //{
    //    public int Compare(ProjectPartListItem p1, ProjectPartListItem p2)
    //    {
    //        if (p1.ProjType.ToString() + p1.ProjName > p2.ProjType)
    //            return 1;
    //        else if (p1.Name.Length < p2.Name.Length)
    //            return -1;
    //        else
    //            return 0;
    //    }
    //}

    //public struct Project : IComparable<Project>
    //{
    //    public int Kind { get; set; }
    //    public int Title { get; set; }
    //    public int Name { get; set; }

    //    public int CompareTo(Project other)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class ProjectPartListItem
    {        
        public int Id { get; set; }

        //public string ProjType { get; set; }
        //public string ProjName { get; set; } 
        public string CompTitle { get; set; } 
        
        //public int? PartType { get; set; }
        public int? UnitTypeId { get; set; }
        
        public decimal Qty { get; set; }
        
        public string Description { get; set; }
        
        public int? StatusId { get; set; }
        
        public string Category { get; set; }
        
        public int? CategoryRank { get; set; }
        
        public int? CategoryId { get; set; }
        
        public decimal? LaborTime { get; set; }

        public decimal? Material { get; set; }

        public decimal? Labor { get; set; }

        public decimal? SubContr { get; set; }

        public decimal? Total
        {
            get
            {
                return Material + Labor + SubContr;
            }
        }

        //public string PartCode { get; set; }

        //public string SuplCode { get; set; }
        public Common.Markup Markup { get; set; }
        public Common.Markup MarkupRate { get; set; }
        public Common.Markup CustomMarkupRate { get; set; }
        public int? Rank { get; set; }

        public bool? IsFavorite { get; set; }

        public static ProjectPartListItem FromBO(CEMVC.Core.BLL.Infrastructure.ProjectPartDetailed part, bool includeMarkup)
        {
            var res = new ProjectPartListItem
            {
                Id = part.Id,
                CompTitle = part.ComponentTitle,
                UnitTypeId = part.TypeId,
                Qty = part.Qty.GetValueOrDefault(),
                Description = part.Title,
                Category = part.CategoryTitle,
                CategoryRank = part.CategoryRank,
                CategoryId = part.CategoryId,
                //ProjName = string.IsNullOrEmpty(part.ComponentTitle) ? "General" : part.ComponentTitle,
                //ProjType = string.IsNullOrEmpty(part.ComponentTitle) ? "0" : (part.ComponentTitle[0] == 'C' ? 1 : 2).ToString() + "_" + part.ComponentTitle,                
                //PartCode = part.PartCode,
                //SuplCode = part.SupplierCode,
                LaborTime = part.LaborHours,
                Material = part.MaterialCost + (includeMarkup ? part.MaterialMarkup.GetValueOrDefault() : 0),
                //RawMaterial = part.MaterialCost,
                Labor = part.LaborCost + (includeMarkup ? part.LaborMarkup.GetValueOrDefault() : 0),
                //RawLabor = part.LaborCost,
                SubContr = part.SubcontractorCost + (includeMarkup ? part.SubcontractorMarkup.GetValueOrDefault() : 0),
                //RawSubCont = part.SubcontractorCost,
                Rank = part.Rank,
                Markup = includeMarkup ? new Common.Markup(part.MaterialMarkup.GetValueOrDefault(), part.LaborMarkup.GetValueOrDefault(),part.SubcontractorMarkup.GetValueOrDefault()) : new Common.Markup(),
                MarkupRate = new Common.Markup(part.MaterialMarkupPercent, part.LaborMarkupPercent, part.SubcontractorMarkupPercent),
                CustomMarkupRate = new Common.Markup(part.IndividualMaterialMarkupPercent, part.IndividualLaborMarkupPercent, part.IndividualSubcontractorMarkupPercent),
                //TotalCost = part.MaterialCost + part.LaborCost + part.SubcontractorCost + (includeMarkup ? part.MaterialMarkup.GetValueOrDefault() + part.LaborMarkup.GetValueOrDefault() + part.SubcontractorMarkup.GetValueOrDefault() : 0)
            };

            return res;
        }
        
    }
}