using CEMVC.Core.BLL.Enums;
using CEMVC.Core.BLL.Infrastructure;
using CEMVC.Core.BLL.Services;
using CEMVC.Core.DAL;
using CEMVC.Utility.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CEMVC.FrontEnd.Web.Models.Common
{
    public struct VendorId
    {
        private bool isEnabled;
        private string partCode;
        private string supplierCode;

        [StringLength(10)]
        public string PartCode { get { return partCode; } set { partCode = value; isEnabled = true; } }
        [StringLength(10)]
        public string SupplierCode { get { return supplierCode; } set { supplierCode = value; isEnabled = true; } }
        public bool Enabled()
        {
            return isEnabled;
        }
    }

    public struct Markup
    {
        private bool isEnabled;
        private decimal? material;
        private decimal? labor;
        private decimal? subcontractor;

        public decimal? Material { get { return material; } set { material = value; isEnabled = true; } }
        public decimal? Labor { get { return labor; } set { labor = value; isEnabled = true; } }
        public decimal? Subcontractor { get { return subcontractor; } set { subcontractor = value; isEnabled = true; } }
        public bool Enabled()
        {
            return isEnabled;
        }

        public Markup(decimal? material, decimal? labor, decimal? subcontractor)
        {
            isEnabled = true;
            this.material = material;
            this.labor = labor;
            this.subcontractor = subcontractor;
        }
    }

    public struct Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Category(Part_Categories bo)
        {
            Id = bo?.id ?? 0;
            Title = bo?.title;
        }
    }

    public class PartEditAdvancedModel
    {
        [Required]
        public int Id { get; set; }

        [StringLength(255, ErrorMessage = "The part description length should be less then 255 characters.")]
        public string Title { get; set; }

        public int? PartTypeId { get; set; }
        [Obsolete]
        public int? UnitType { get { return PartTypeId; } }
        public PartTypeModel PartType { get; private set; }

        public decimal Qty { get; set; }
        public int? ProjectId { get; set; }

        #region Vendor
        public VendorId Vendor { get; set; }
        #endregion

        #region Report Texts fields

        public string PreliminaryText { get; set; }
        public string FormalText { get; set; }
        public string SubcontractorText { get; set; }
        #endregion

        #region Material Cost variables

        public bool MaterialCostEnabled { get; set; }
        public int MaterialCostFormula { get; set; }
        public string MaterialCostFormulaLabel { get; private set; }
        public decimal? UnitCost { get; set; }
        public double? MaterialMultiplier { get; set; }
        //public decimal? MaterialMarkup { get; set; }
        #endregion

        #region Labor Cost variables

        public bool LaborCostEnabled { get; set; }
        public int LaborCostFormula { get; set; }
        public string LaborCostFormulaLabel { get; private set; }
        public string LaborRateLetter { get; set; }
        public double? LaborRate { get; set; }
        public Labor_RatesModel LaborRateModel { get; private set; }
        /// <summary>
        /// Cost of Labor per Unit
        /// </summary>
        public decimal? LaborCostPerUnit { get; set; }
        public double? HourlyMultiplier { get; set; }
        //public decimal? LaborMarkup { get; set; }
        #endregion

        #region Subcontractor Cost variables

        public bool SubcontractorCostEnabled { get; set; }
        public int SubcontractorCostFormula { get; set; }
        public string SubcontractorCostFormulaLabel { get; private set; }
        public decimal? SubcontractorCost { get; set; }
        public double? SubcontractorMultiplier { get; set; }
        public string SubcontractorLaborRateLetter { get; set; }
        public Labor_RatesModel SubcontractorLaborRateModel { get; private set; }
        #endregion

        #region markup
        public Markup Markup { get; set; }
        #endregion

        [Obsolete]
        public bool? IsSubcontrAmount { get; set; }
        [Obsolete]
        public double? RoughInHours { get; set; }
        [Obsolete]
        public double? FinishHours { get; set; }
        [Obsolete]
        public int? RoughInPercent { get; set; }
        [Obsolete]
        public int? FinishPercent { get; set; }

        #region Dashbord variables
        //public string CategoryTitle { get; private set; }
        //public string SubcategoryTitle { get; private set; }
        //public string GroupTitle { get; private set; }
        public bool? IsFavorite { get; private set; }
        public int? OriginalPartId { get; private set; }
        public Category Category { get; set; }
        public Category SubCategory { get; set; }
        public Category Group { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? GroupId { get; set; }
        #endregion

        #region Miscellaneous variables
        public bool HasImageLoaded { get; private set; }
        public string NotesText { get; set; }
        #endregion

        public PartEditAdvancedModel() { }

        public PartEditAdvancedModel(Project_Parts part, IPartService parts = null, IPartCategoryService categories = null) 
        {
            Id = part.id;
            Qty = part.qty ?? 1m;
            Title = !string.IsNullOrWhiteSpace(part.part_title) ? part.part_title : part.Part.Parts_Title_Cache.title;
            ProjectId = part.project_id;

            if (part.calculation_params != 0)
            {

                MaterialCostEnabled = (ProjectPartCostCalculationTypeEnum.MaterialCostEnabled & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0;
                MaterialCostFormula = MaterialCostEnabled ? //;
                    (int)((ProjectPartCostCalculationTypeEnum.MaterialCostInDollarsPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? MaterialCostCalculationTypeEnum.DollarPerUnit : MaterialCostCalculationTypeEnum.LumpSum) : (int)MaterialCostCalculationTypeEnum.DollarPerUnit;

                LaborCostEnabled = (ProjectPartCostCalculationTypeEnum.LaborCostEnabled & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0;
                LaborCostFormula = LaborCostEnabled ? (int)(//
                    (ProjectPartCostCalculationTypeEnum.LaborCostInHoursPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? LaborCostCalculationTypeEnum.HoursPerUnit :
                    (ProjectPartCostCalculationTypeEnum.LaborCostInDollarsPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? LaborCostCalculationTypeEnum.DollarPerUnit :
                    (ProjectPartCostCalculationTypeEnum.LaborCostInHours & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? LaborCostCalculationTypeEnum.Hours : LaborCostCalculationTypeEnum.LumpSum) : (int)LaborCostCalculationTypeEnum.HoursPerUnit;

                SubcontractorCostEnabled = (ProjectPartCostCalculationTypeEnum.SubcontractorCostEnabled & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0;
                SubcontractorCostFormula = SubcontractorCostEnabled ? (int)(//
                    (ProjectPartCostCalculationTypeEnum.SubcontractorCostInHoursPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? SubcontractorCostCalculationTypeEnum.HoursPerUnit :
                    (ProjectPartCostCalculationTypeEnum.SubcontractorCostInDollarsPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? SubcontractorCostCalculationTypeEnum.DollarPerUnit : SubcontractorCostCalculationTypeEnum.LumpSum) : (int)SubcontractorCostCalculationTypeEnum.HoursPerUnit;

            }
            else
                SetDetaultCalculationParams(part);

            PartTypeId = PartTypeId ?? part.type_id;
            PartType = DropDownItems.PartTypes.FirstOrDefault(t => t.Id == PartTypeId);

            //Material Cost
            MaterialCostFormulaLabel = ((MaterialCostCalculationTypeEnum)MaterialCostFormula).GetDescription(useNameIfEmpty: true);
            UnitCost = part.unit_cost.GetValueOrDefault();
            MaterialMultiplier = part.material_multiplier;
            //MaterialMarkup = part.material_markup;

            //Labor Cost
            LaborCostFormulaLabel = ((LaborCostCalculationTypeEnum)LaborCostFormula).GetDescription(useNameIfEmpty: true);
            LaborRateLetter = part.labor_rate_letter; // ?? DropDownItems.LaborRates.First().Letter;
            LaborRateModel = !string.IsNullOrEmpty(LaborRateLetter)
                ? DropDownItems.UserLaborRates().FirstOrDefault(x => x.Letter == LaborRateLetter) ??  new Labor_RatesModel() {Letter = LaborRateLetter}
                : new Labor_RatesModel();

            LaborRate = part.labor_rate ?? (double?)LaborRateModel.Rate; //Labor Rate is initialized after LaborRateModel!
            LaborCostPerUnit = part.labor_cost_per_unit;
            HourlyMultiplier = part.hourly_multiplier;
            //LaborMarkup = part.labor_markup;

            //Subcontractor cost
            SubcontractorCostFormulaLabel = ((SubcontractorCostCalculationTypeEnum)SubcontractorCostFormula).GetDescription(useNameIfEmpty: true);
            SubcontractorCost = part.subcontractor_cost;
            SubcontractorMultiplier = part.subcontractor_multiplier;
            SubcontractorLaborRateLetter = part.subcontractor_labor_rate_letter ?? part.labor_rate_letter;
            SubcontractorLaborRateModel = !string.IsNullOrEmpty(SubcontractorLaborRateLetter)
                ? DropDownItems.UserLaborRates().FirstOrDefault(x => x.Letter == SubcontractorLaborRateLetter) ?? new Labor_RatesModel() { Letter = LaborRateLetter }
                : new Labor_RatesModel();

            //markup
            Markup = new Markup(part.material_markup, part.labor_markup, part.subcontractor_markup);

            //Report Texts
            PreliminaryText = part.preliminary;
            FormalText = part.formal;
            SubcontractorText = part.subcontractor;

            Category = part.category_id.HasValue ? new Category(categories.GetById(part.category_id.Value)) : new Category();
            //CategoryTitle = part.category_id.HasValue ? categories.GetById(part.category_id.Value)?.title : null;
            SubCategory = part.subcategory_id.HasValue ? new Category(categories.GetById(part.subcategory_id.Value)) : new Category();
            Group = part.group_id.HasValue ? new Category(categories.GetById(part.group_id.Value)) : new Category();

            HasImageLoaded = part.image_id.HasValue;
            NotesText = part.notes;

            OriginalPartId = part.part_id;
            if (part.is_custom == false && OriginalPartId.HasValue)
                IsFavorite = parts.GetById(part.part_id.Value)?.is_favorite;
        }

        public PartEditAdvancedModel(PartExt part, IPartCategoryService categories = null)
        {
            Id = part.id;
            Qty = 1m;
            Title = part.title;
            Vendor = new VendorId
            {
                PartCode = part.part_code,
                SupplierCode = part.supplier_code
            };

            if (part.calculation_params != 0)
            {
                MaterialCostEnabled = (ProjectPartCostCalculationTypeEnum.MaterialCostEnabled & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0;
                MaterialCostFormula = MaterialCostEnabled ? //;
                    (int)((ProjectPartCostCalculationTypeEnum.MaterialCostInDollarsPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? MaterialCostCalculationTypeEnum.DollarPerUnit : MaterialCostCalculationTypeEnum.LumpSum) : (int)MaterialCostCalculationTypeEnum.DollarPerUnit;

                LaborCostEnabled = (ProjectPartCostCalculationTypeEnum.LaborCostEnabled & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0;
                LaborCostFormula = LaborCostEnabled ? (int)(//
                    (ProjectPartCostCalculationTypeEnum.LaborCostInHoursPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? LaborCostCalculationTypeEnum.HoursPerUnit :
                    (ProjectPartCostCalculationTypeEnum.LaborCostInDollarsPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? LaborCostCalculationTypeEnum.DollarPerUnit :
                    (ProjectPartCostCalculationTypeEnum.LaborCostInHours & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? LaborCostCalculationTypeEnum.Hours : LaborCostCalculationTypeEnum.LumpSum) : (int)LaborCostCalculationTypeEnum.HoursPerUnit;

                SubcontractorCostEnabled = (ProjectPartCostCalculationTypeEnum.SubcontractorCostEnabled & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0;
                SubcontractorCostFormula = SubcontractorCostEnabled ? (int)(//
                    (ProjectPartCostCalculationTypeEnum.SubcontractorCostInHoursPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? SubcontractorCostCalculationTypeEnum.HoursPerUnit :
                    (ProjectPartCostCalculationTypeEnum.SubcontractorCostInDollarsPerUnit & (ProjectPartCostCalculationTypeEnum)part.calculation_params) != 0 ? SubcontractorCostCalculationTypeEnum.DollarPerUnit : SubcontractorCostCalculationTypeEnum.LumpSum) : (int)SubcontractorCostCalculationTypeEnum.HoursPerUnit;

            }
            else
                SetDetaultCalculationParams(part);

            PartTypeId = PartTypeId ?? part.type_id ?? 1;
            PartType = DropDownItems.PartTypes.FirstOrDefault(t => t.Id == PartTypeId);

            //Material Cost
            MaterialCostFormulaLabel = ((MaterialCostCalculationTypeEnum)MaterialCostFormula).GetDescription(useNameIfEmpty: true);
            UnitCost = part.unit_cost.GetValueOrDefault();
            MaterialMultiplier = part.material_multiplier;

            //Labor Cost
            LaborCostFormulaLabel = ((LaborCostCalculationTypeEnum)LaborCostFormula).GetDescription(useNameIfEmpty: true);
            LaborRateLetter = part.labor_rate_letter;
            LaborRateModel = !string.IsNullOrEmpty(LaborRateLetter)
                ? DropDownItems.UserLaborRates().FirstOrDefault(x => x.Letter == LaborRateLetter) ?? new Labor_RatesModel() { Letter = LaborRateLetter }
                : DropDownItems.UserLaborRates().FirstOrDefault() ?? new Labor_RatesModel();

            LaborRate = part.labor_rate ?? (double?)LaborRateModel.Rate; //Labor Rate is initialized after LaborRateModel!
            LaborCostPerUnit = part.labor_cost_per_unit;
            HourlyMultiplier = part.hourly_multiplier;

            //Subcontractor cost
            SubcontractorCostFormulaLabel = ((SubcontractorCostCalculationTypeEnum)SubcontractorCostFormula).GetDescription(useNameIfEmpty: true);
            SubcontractorCost = part.subcontractor_cost;
            SubcontractorMultiplier = part.subcontractor_multiplier;
            SubcontractorLaborRateLetter = part.subcontractor_labor_rate_letter ?? part.labor_rate_letter;
            SubcontractorLaborRateModel = !string.IsNullOrEmpty(SubcontractorLaborRateLetter)
                ? DropDownItems.UserLaborRates().FirstOrDefault(x => x.Letter == SubcontractorLaborRateLetter) ?? new Labor_RatesModel() { Letter = LaborRateLetter }
                : DropDownItems.UserLaborRates().FirstOrDefault() ?? new Labor_RatesModel();

            //Report Texts
            PreliminaryText = part.preliminary;
            FormalText = part.formal;
            SubcontractorText = part.subcontractor;

            Category = part.category_id.HasValue ? new Category(categories.GetById(part.category_id.Value)) : new Category();
            SubCategory = part.subcategory_id.HasValue ? new Category(categories.GetById(part.subcategory_id.Value)) : new Category();
            Group = part.group_id.HasValue ? new Category(categories.GetById(part.group_id.Value)) : new Category();

            HasImageLoaded = part.image_id.HasValue;
            NotesText = part.notes;

            OriginalPartId = part.id;
            IsFavorite = part.is_favorite;
        }

        public PartEditAdvancedModel(int categoryId, IPartCategoryService categories)
        {
            Id = 0;
            Qty = 1m;
            Title = "";
            PartTypeId = 1;
            PartType = DropDownItems.PartTypes.FirstOrDefault(t => t.Id == PartTypeId);
            Vendor = new VendorId
            {
                PartCode = "",
                SupplierCode = ""
            };

            //Material Cost
            MaterialCostEnabled = false;
            MaterialCostFormula = (int)MaterialCostCalculationTypeEnum.DollarPerUnit;

            //Labor Cost
            LaborCostEnabled = false;
            LaborCostFormula = (int)LaborCostCalculationTypeEnum.HoursPerUnit;

            //Subcontractor cost
            SubcontractorCostEnabled = false;
            SubcontractorCostFormula = (int)SubcontractorCostCalculationTypeEnum.DollarPerUnit;

            //Material Cost
            MaterialCostFormulaLabel = ((MaterialCostCalculationTypeEnum)MaterialCostFormula).GetDescription(useNameIfEmpty: true);
            UnitCost = 0m;
            MaterialMultiplier = 1;

            //Labor Cost
            LaborCostFormulaLabel = ((LaborCostCalculationTypeEnum)LaborCostFormula).GetDescription(useNameIfEmpty: true);
            LaborRateModel = DropDownItems.UserLaborRates().FirstOrDefault();
            if (LaborRateModel != null)
            {
                LaborRateLetter = LaborRateModel.Letter;
                LaborRate = (double?)LaborRateModel.Rate; //Labor Rate is initialized after LaborRateModel!
            }
            else
                LaborRateModel = new Labor_RatesModel();
            LaborCostPerUnit = 0;
            HourlyMultiplier = 1;

            //Subcontractor cost
            SubcontractorCostFormulaLabel = ((SubcontractorCostCalculationTypeEnum)SubcontractorCostFormula).GetDescription(useNameIfEmpty: true);
            SubcontractorCost = 0;
            SubcontractorMultiplier = 1;
            SubcontractorLaborRateModel = DropDownItems.UserLaborRates().FirstOrDefault();

            Category = new Category(categories.GetById(categoryId));
            SubCategory = new Category(categories.GetChildCategories(categoryId).FirstOrDefault(x => x.is_default) ?? new Part_Categories());
            Group = new Category(categories.GetChildCategories(SubCategory.Id).FirstOrDefault(x => x.is_default) ?? new Part_Categories()); ;

            OriginalPartId = 0;
            IsFavorite = null; //must be null
        }

        public void UpdateBo(Project_Parts part)
        {
            part.qty = Qty;
            part.part_title = Title;
            part.type_id = PartTypeId;

            //Material Cost
            if (MaterialCostEnabled)
            {
                part.unit_cost = UnitCost;
                part.material_multiplier = (PartTypeId == (int)PartTypeEnum.Pound) ? 1 : MaterialMultiplier;
            }
            else
            {
                part.unit_cost = null;
                part.material_multiplier = null;
                //part.material_markup = null;
                //MaterialMarkup = null;
            }

            if (LaborCostEnabled)
            {
                part.labor_cost_per_unit = LaborCostPerUnit;
                part.labor_rate_letter = LaborRateLetter;
                part.labor_rate = LaborRate;
                part.hourly_multiplier = (PartTypeId == (int)PartTypeEnum.Pound) ? 0 : HourlyMultiplier;
            }
            else
            {
                part.labor_cost_per_unit = null;
                part.labor_rate_letter = null;
                part.labor_rate = null;
                part.hourly_multiplier = (PartTypeId == (int)PartTypeEnum.Pound) ? 0 : HourlyMultiplier;

                //LaborMarkup = null;
            }

            if (SubcontractorCostEnabled)
            {
                part.subcontractor_cost = SubcontractorCost;
                part.subcontractor_multiplier = SubcontractorMultiplier;
                part.subcontractor_labor_rate_letter = SubcontractorLaborRateLetter;
            }
            else
            {
                part.subcontractor_cost = null;
                part.subcontractor_multiplier = null;
                part.subcontractor_labor_rate_letter = null;
            }

            //Calclucation parameters
            var calcParam = (int)ProjectPartCostCalculationTypeEnum.CostCalculationEnabled;
            if (MaterialCostEnabled)
                calcParam |= (int)ProjectPartCostCalculationTypeEnum.MaterialCostEnabled | (int)((MaterialCostCalculationTypeEnum)MaterialCostFormula).GetCorrespondingValue().GetValueOrDefault(0);
            if (LaborCostEnabled)
                calcParam |= (int)ProjectPartCostCalculationTypeEnum.LaborCostEnabled | (int)((LaborCostCalculationTypeEnum)LaborCostFormula).GetCorrespondingValue().GetValueOrDefault(0);
            if (SubcontractorCostEnabled)
                calcParam |= (int)ProjectPartCostCalculationTypeEnum.SubcontractorCostEnabled | (int)((SubcontractorCostCalculationTypeEnum)SubcontractorCostFormula).GetCorrespondingValue().GetValueOrDefault(0);

            part.calculation_params = calcParam;

            //Markups
            part.material_markup = Markup.Material;
            part.labor_markup = Markup.Labor;
            part.subcontractor_markup = Markup.Subcontractor;

            //Report texts
            part.preliminary = PreliminaryText;
            part.formal = FormalText;
            part.subcontractor = SubcontractorText;
            part.notes = NotesText;

        }

        public void UpdateBo(PartExt part)
        {
            //part.category_id = category != null ? category.id : part.category_id;
            part.title = Title;
            part.type_id = PartTypeId;
            part.part_code = Vendor.PartCode;
            part.supplier_code = Vendor.SupplierCode;

            //Material Cost
            if (MaterialCostEnabled)
            {
                part.unit_cost = UnitCost;
                part.material_multiplier = (PartTypeId == (int)PartTypeEnum.Pound) ? 1 : MaterialMultiplier;
            }
            else
            {
                part.unit_cost = null;
                part.material_multiplier = null;
            }

            if (LaborCostEnabled)
            {
                part.labor_cost_per_unit = LaborCostPerUnit;
                part.labor_rate_letter = LaborRateLetter;
                part.labor_rate = LaborRate;
                part.hourly_multiplier = (PartTypeId == (int)PartTypeEnum.Pound) ? 0 : HourlyMultiplier;
            }
            else
            {
                //part.labor_rate_letter = null; //do not reset Letter
                part.labor_rate = null;
                part.labor_cost_per_unit = null;
                part.hourly_multiplier = (PartTypeId == (int)PartTypeEnum.Pound) ? 0 : HourlyMultiplier;
            }

            if (SubcontractorCostEnabled)
            {
                part.subcontractor_cost = SubcontractorCost;
                part.subcontractor_multiplier = SubcontractorMultiplier;
                part.subcontractor_labor_rate_letter = SubcontractorLaborRateLetter;
            }
            else
            {
                part.subcontractor_cost = null;
                part.subcontractor_multiplier = null;
                part.subcontractor_labor_rate_letter = null;
            }

            //Calclucation parameters
            var calcParam = (int)ProjectPartCostCalculationTypeEnum.CostCalculationEnabled;
            if (MaterialCostEnabled)
                calcParam |= (int)ProjectPartCostCalculationTypeEnum.MaterialCostEnabled | (int)((MaterialCostCalculationTypeEnum)MaterialCostFormula).GetCorrespondingValue().GetValueOrDefault(0);
            if (LaborCostEnabled)
                calcParam |= (int)ProjectPartCostCalculationTypeEnum.LaborCostEnabled | (int)((LaborCostCalculationTypeEnum)LaborCostFormula).GetCorrespondingValue().GetValueOrDefault(0);
            if (SubcontractorCostEnabled)
                calcParam |= (int)ProjectPartCostCalculationTypeEnum.SubcontractorCostEnabled | (int)((SubcontractorCostCalculationTypeEnum)SubcontractorCostFormula).GetCorrespondingValue().GetValueOrDefault(0);

            part.calculation_params = calcParam;

            //Report texts
            part.preliminary = PreliminaryText;
            part.formal = FormalText;
            part.subcontractor = SubcontractorText;
            part.notes = NotesText;

            //Categories
            part.category_id = CategoryId ?? part.category_id;
            part.subcategory_id = SubCategoryId ?? part.subcategory_id;
            part.group_id = GroupId ?? part.group_id;
        }
        /// <summary>
        /// Init form when opened 1st time
        /// </summary>
        public void SetDetaultCalculationParams(Project_Parts part)
        {
            //Material Cost
            MaterialCostEnabled = part.is_subcontr_amount != true && part.type_id.HasValue && part.type_id != (int)PartTypeEnum.Hour && part.unit_cost.HasValue;
            MaterialCostFormula = (int)MaterialCostCalculationTypeEnum.DollarPerUnit;

            //Labor Cost
            LaborCostEnabled = part.is_subcontr_amount != true && part.type_id.HasValue && part.type_id != (int)PartTypeEnum.Pound && part.hourly_multiplier.HasValue;
            LaborCostFormula = (int)LaborCostCalculationTypeEnum.HoursPerUnit;

            //Subcontractor cost
            SubcontractorCostEnabled = part.is_subcontr_amount == true;
            SubcontractorCostFormula = (int)SubcontractorCostCalculationTypeEnum.DollarPerUnit;

            if (SubcontractorCostEnabled)
                if (part.type_id == (int)PartTypeEnum.Pound)
                {
                    PartTypeId = part.type_id = (int)PartTypeEnum.Piece;
                    part.subcontractor_cost = 1;
                    part.subcontractor_multiplier = 1;
                }
                else if (part.type_id == (int)PartTypeEnum.Hour)
                {
                    PartTypeId = part.type_id = (int)PartTypeEnum.Piece;
                    SubcontractorCostFormula = (int)SubcontractorCostCalculationTypeEnum.HoursPerUnit;
                    part.subcontractor_cost = 1;
                }
                else
                {
                    part.subcontractor_cost = part.unit_cost;
                    part.subcontractor_multiplier = part.material_multiplier;
                }
        }

        /// <summary>
        /// Init form when opened 1st time
        /// </summary>
        public void SetDetaultCalculationParams(Part part)
        {
            //Material Cost
            MaterialCostEnabled = part.is_subcontr_amount != true && part.type_id.HasValue && part.type_id != (int)PartTypeEnum.Hour && part.unit_cost.HasValue;
            MaterialCostFormula = (int)MaterialCostCalculationTypeEnum.DollarPerUnit;

            //Labor Cost
            LaborCostEnabled = part.is_subcontr_amount != true && part.type_id.HasValue && part.type_id != (int)PartTypeEnum.Pound && part.hourly_multiplier.HasValue;
            LaborCostFormula = (int)LaborCostCalculationTypeEnum.HoursPerUnit;

            //Subcontractor cost
            SubcontractorCostEnabled = part.is_subcontr_amount == true;
            SubcontractorCostFormula = (int)SubcontractorCostCalculationTypeEnum.DollarPerUnit;

            if (SubcontractorCostEnabled)
                if (part.type_id == (int)PartTypeEnum.Pound)
                {
                    PartTypeId = part.type_id = (int)PartTypeEnum.Piece;
                    part.subcontractor_cost = 1;
                    part.subcontractor_multiplier = 1;
                }
                else if (part.type_id == (int)PartTypeEnum.Hour)
                {
                    PartTypeId = part.type_id = (int)PartTypeEnum.Piece;
                    SubcontractorCostFormula = (int)LaborCostCalculationTypeEnum.HoursPerUnit;
                    part.subcontractor_cost = 1;
                }
                else
                {
                    part.subcontractor_cost = part.unit_cost;
                    part.subcontractor_multiplier = part.material_multiplier;
                }
        }

        public static PartEditAdvancedModel CreateCustomPartEditModel(int projectId, string title, decimal? unitCost, decimal? laborCost)
        {
            var res = new PartEditAdvancedModel
            {
                //Id = 0,
                PartTypeId = 1,
                ProjectId = projectId,
                Qty = 1m,
                Title = title,
                Markup = new Markup(null, null, null),

                //Material Cost
                MaterialCostEnabled = unitCost.HasValue,
                MaterialCostFormula = unitCost.HasValue ? (int)MaterialCostCalculationTypeEnum.LumpSum : (int)MaterialCostCalculationTypeEnum.DollarPerUnit,
                MaterialMultiplier = 1,
                UnitCost = unitCost,

                //Labor Cost
                LaborCostEnabled = laborCost.HasValue,
                LaborCostFormula = laborCost.HasValue ? (int)LaborCostCalculationTypeEnum.LumpSum : (int)LaborCostCalculationTypeEnum.HoursPerUnit,
                LaborCostPerUnit = laborCost,
                LaborRateModel = DropDownItems.UserLaborRates().FirstOrDefault() ?? new Labor_RatesModel(),
                HourlyMultiplier = 1,

                //Subcontractor cost
                SubcontractorCostFormula = (int)SubcontractorCostCalculationTypeEnum.DollarPerUnit,
                SubcontractorCost = 0,
                SubcontractorMultiplier = 1,
                SubcontractorLaborRateModel = new Labor_RatesModel()
            };

            res.PartType = DropDownItems.PartTypes.FirstOrDefault(t => t.Id == res.PartTypeId);
            res.LaborRate = (double?)res.LaborRateModel.Rate;
            res.LaborRateLetter = res.LaborRateModel.Letter;
            res.SubcontractorLaborRateLetter = res.LaborRateLetter;
            res.SubcontractorLaborRateModel = new Labor_RatesModel { Rate = (decimal?)res.LaborRate, Letter = res.LaborRateLetter, Title = res.LaborRateModel.Title };

            res.MaterialCostFormulaLabel = ((MaterialCostCalculationTypeEnum)res.MaterialCostFormula).GetDescription(useNameIfEmpty: true);
            res.LaborCostFormulaLabel = ((LaborCostCalculationTypeEnum)res.LaborCostFormula).GetDescription(useNameIfEmpty: true);
            res.SubcontractorCostFormulaLabel = ((SubcontractorCostCalculationTypeEnum)res.SubcontractorCostFormula).GetDescription(useNameIfEmpty: true);

            return res;
        }
    }
}