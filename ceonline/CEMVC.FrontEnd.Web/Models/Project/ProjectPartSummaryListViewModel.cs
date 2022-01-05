using CEMVC.Core.BLL.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CEMVC.FrontEnd.Web.Models.Project
{
    public class ProjectPartSummaryListViewModel
    {
        #region Properties
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Category")]
        public int? CategoryRank { get; set; }

        [Display(Name = "Lab. Hours")]
        public double? LabHours { get; set; }

        [Display(Name = "Labor")]
        public double? Labor { get; set; }

        [Display(Name = "Material")]
        public double? Material { get; set; }

        [Display(Name = "Co.Cost")]
        public double? CoCost { get; set; }

        [Display(Name = "RI.Hrs")]
        public double? RoughHours { get; set; }

        [Display(Name = "Fin.Hrs")]
        public Double FinishHours { get; set; }

        [Display(Name = "Sub. Cost")]
        public double? SubCost { get; set; }

        [Display(Name = "Total")]
        public double? Total
        {
            get
            {
                return Labor.GetValueOrDefault() + Material.GetValueOrDefault() + SubCost.GetValueOrDefault();
            }
        }

        public decimal MarkupLab { get; set; }
        public decimal MarkupMat { get; set; }
        public decimal MarkupSubctr { get; set; }

        public double? material_cost{ get; set; }
        public double? alt_sum_materials { get; set; }
        public double? alt_markup_materials { get; set; }
        public double? labor_cost { get; set; }
        public double? alt_sum_labor { get; set; }
        public double? alt_markup_labor { get; set; }
        public double? subcont_unit_cost { get; set; }
        public double? alt_sum_subcont { get; set; }
        public double? alt_markup_subcont { get; set; }
        public double? total_cost { get; set; }
        public double? company_unit_cost { get; set; }

        public double? material_cost_wmp { get; set; }
        public double? labor_cost_wmp { get; set; }
        public double? company_unit_cost_wmp { get; set; }
        public double? subcont_unit_cost_wmp { get; set; }
        public double? total_cost_wmp { get; set; }

        #endregion

        public ProjectPartSummaryListViewModel()
        {
            
        }
        public ProjectPartSummaryListViewModel(CEMVC.Core.DAL.get_project_categories_Result project_category)
        {
            Id = project_category.category_id;
            Category = project_category.category_title;
            CategoryRank = project_category.category_rank;
            LabHours = Math.Round((project_category.install_hours ?? 0) * 10, MidpointRounding.AwayFromZero) / 10;
            Labor = project_category.labor_cost ?? 0;
            Material = project_category.material_cost ?? 0;
            SubCost = project_category.subcont_unit_cost ?? 0;
            RoughHours = project_category.rough_hours ?? 0;
            FinishHours = project_category.finish_hours ?? 0;
            //Total = 1;

            material_cost = project_category.material_cost ?? 0;
            alt_sum_materials = project_category.alt_sum_materials ?? 0;
            alt_markup_materials = project_category.alt_markup_materials ?? 0;
            labor_cost = project_category.labor_cost ?? 0;
            alt_sum_labor = project_category.alt_sum_labor ?? 0;
            alt_markup_labor = project_category.alt_markup_labor ?? 0;
            subcont_unit_cost = project_category.subcont_unit_cost ?? 0;
            alt_sum_subcont = project_category.alt_sum_subcont ?? 0;
            alt_markup_subcont = project_category.alt_markup_subcont ?? 0;
            total_cost = project_category.total_cost ?? 0;
            company_unit_cost = project_category.company_unit_cost ?? 0;

        }

        public ProjectPartSummaryListViewModel(CEMVC.Core.DAL.get_project_categories_Result projectPart,
            double? markupMaterials, double? markupLabor, double? markupSubcont, int? markupMaterialsPercent,
            int? markupLaborPercent, int? markupSubcontPercent, double? materialTotalNet, double? laborTotalNet,
            double? subContTotalNet, bool? includeMarkup)
            : this(projectPart)
        {
            var markupMaterialsChange = materialTotalNet == 0 || materialTotalNet == null
                ? markupMaterialsPercent/100
                : markupMaterials/materialTotalNet;
            var markupLaborChange = laborTotalNet == 0 || laborTotalNet == null
                ? markupLaborPercent/100
                : markupLabor/laborTotalNet;
            var markupSubcontChange = subContTotalNet == 0 || subContTotalNet == null
                ? markupSubcontPercent/100
                : markupSubcont/subContTotalNet;

            var labcostNet = labor_cost;
            var matcostNet = material_cost;
            var cocostNet = company_unit_cost;
            var subcostNet = subcont_unit_cost;
            var totalcostNet = total_cost;

            var altSumMaterials = alt_sum_materials;
            var altSumLabor = alt_sum_labor;
            var altSumSubCost = alt_sum_subcont;

            var labcost = (labcostNet - altSumLabor) * (1 + markupLaborChange) + altSumLabor + alt_markup_labor;
            var matcost = (matcostNet - altSumMaterials) * (1 + markupMaterialsChange) + altSumMaterials + alt_markup_materials;
            var cocost = matcost + labcost;
            var subcost = (subcont_unit_cost - altSumSubCost) * (1 + markupSubcontChange) + altSumSubCost + alt_markup_subcont;
            var totalcost = cocost + subcost;

            material_cost_wmp = matcost;
            company_unit_cost_wmp = cocost;
            subcont_unit_cost_wmp = subcost;
            total_cost_wmp = totalcost;
            labor_cost_wmp = labcost;

            if (includeMarkup.HasValue && includeMarkup.Value) {
                Labor = labcost;
                Material = matcost;
                CoCost = cocost;
                SubCost = subcost;
                //Total = totalcost;
            } else {
                Labor = labcostNet;
                Material = matcostNet;
                CoCost = cocostNet;
                SubCost = subcostNet;
                //Total = totalcostNet;
            }
        }

        #region convertors
        public static IEnumerable<ProjectPartSummaryListViewModel> FromBOCollection(IEnumerable<ProjectPartCategorySummarized> items, bool includeMarkup)
        {
            return items.Select(x => new ProjectPartSummaryListViewModel
            {
                Id = x.Id,
                Category = x.Title,
                CategoryRank = x.Rank,
                LabHours = (double)x.LaborHours,
                Labor = (double)(x.LaborCost + (includeMarkup ? x.LaborMarkup : 0)),
                Material = (double)(x.MaterialCost + (includeMarkup ? x.MaterialMarkup : 0)),
                SubCost = (double)(x.SubcontractorCost + (includeMarkup ? x.SubcontractorMarkup : 0)),
                CoCost = (double)(x.CompoundCost + (includeMarkup ? x.LaborMarkup + x.MaterialMarkup : 0)),

                material_cost = (double)x.MaterialCost,
                material_cost_wmp = (double)(x.MaterialCost + x.MaterialMarkup),
                company_unit_cost = (double)x.CompoundCost,
                company_unit_cost_wmp = (double)(x.CompoundCost + x.LaborMarkup + x.MaterialMarkup),
                subcont_unit_cost = (double)x.SubcontractorCost,
                subcont_unit_cost_wmp = (double)(x.SubcontractorCost + x.SubcontractorMarkup),
                labor_cost = (double)x.LaborCost,
                labor_cost_wmp = (double)(x.LaborCost + x.LaborMarkup),
                total_cost = (double)x.TotalCost,
                total_cost_wmp = (double)(x.TotalCost + x.TotalMarkup),

                MarkupLab = x.LaborMarkup,
                MarkupMat = x.MaterialMarkup,
                MarkupSubctr = x.SubcontractorMarkup
            });
        }

        public static ProjectPartSummaryListViewModel FromBO(CEMVC.Core.DAL.get_project_categories_Result project_part, double? markupMaterials, double? markupLabor, double? markupSubcont, int? markupMaterialsPercent, int? markupLaborPercent, int? markupSubcontPercent, double? totalMaterials, double? totalLabor, double? totalSubcont, bool? includeMarkup)
        {
            ProjectPartSummaryListViewModel model = null;
            if (project_part != null)
            {
                model = new ProjectPartSummaryListViewModel(project_part, markupMaterials, markupLabor, markupSubcont, markupMaterialsPercent, markupLaborPercent, markupSubcontPercent, totalMaterials, totalLabor, totalSubcont, includeMarkup);
            }
            return model;
        }

        public static ICollection<ProjectPartSummaryListViewModel> FromBoCollection(IEnumerable<CEMVC.Core.DAL.get_project_categories_Result> collection, double? markupMaterials, double? markupLabor, double? markupSubcont, int? markupMaterialsPercent, int? markupLaborPercent, int? markupSubcontPercent, double? totalMaterials, double? totalLabor, double? totalSubcont, bool? includeMarkup)
        {
            var newModelCollection = new List<ProjectPartSummaryListViewModel>();
            if (collection != null)
                newModelCollection.AddRange(collection.OrderBy(x => x.category_rank).Select(x => FromBO(x, markupMaterials, markupLabor, markupSubcont, markupMaterialsPercent, markupLaborPercent, markupSubcontPercent, totalMaterials, totalLabor, totalSubcont, includeMarkup)));

            return newModelCollection;
        }

        #endregion
    }

}