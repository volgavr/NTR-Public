using System;
using System.Linq;
using CEMVC.Core.BLL.Enums;
using CEMVC.Core.DAL.RemodelMAX;

namespace CEMVC.MasterData.BLL.Services
{
    public interface IMasterPartCategoryService
    {
        IQueryable<Part_Category> GetAllCategories();
        IQueryable<Part_Category> GetAllDefaultCategories();

        [Obsolete]
        IQueryable<Part_Category> GetChildCategories(int? parentId);
        IQueryable<Part_Category> GetChildCategories(Guid? parentUid);
        Part_Category GetChildDefaultCategory(Guid parentUid);
        [Obsolete]
        Part_Category GetChildDefaultCategory(int parentId);

        Part_Category GetByUid(Guid uid);
        [Obsolete]
        Part_Category GetById(int id);

        int AddNewDefaultPartCategory(Part_Category category, bool? insertABottom);
        int AddNewDefaultPartSubCategory(Part_Category subcategory, bool? insertABottom);
        int AddNewDefaultPartGroup(Part_Category group, bool? insertABottom);

        [Obsolete]
        void UpdateParts(int category_id, int? parent_category_id, PartCategoryLevelEnum level);
        void UpdateParts(Guid category_uid, Guid? parent_category_uid, PartCategoryLevelEnum level);
        void SaveChangesPartCategory(Part_Category category);

        [Obsolete]
        void MoveCategory(int id, int? parent_id, int direction, int rank);
        void MoveCategory(Guid uid, Guid? parent_uid, int direction, int rank);
    }
}