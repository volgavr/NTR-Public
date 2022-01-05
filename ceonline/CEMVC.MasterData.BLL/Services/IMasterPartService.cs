using System;
using System.Linq;
using CEMVC.Core.BLL.Enums;
using CEMVC.Core.DAL.RemodelMAX;

namespace CEMVC.MasterData.BLL.Services
{
    public interface IMasterPartService
    {     
        IQueryable<Part> GetAllParts(bool includeDeleted = false);
        //IQueryable<Part> GetAllDefaultParts(bool includeDeleted = false);
        IQueryable<Part> GetPartsByCategoryId(Guid id, PartCategoryLevelEnum level = PartCategoryLevelEnum.MainCategory);
        [Obsolete]
        IQueryable<Part> GetPartsByCategoryIds(int category_id, int subcategory_id, int group_id);
        IQueryable<Part> GetPartsByCategoryIds(Guid category_uid, Guid subcategory_uid, Guid group_uid);
        [Obsolete]
        Part GetById(int id);
        Part GetByUid(Guid uid);
        [Obsolete]
        void RemovePart(int id);
        void RemovePart(Guid uid);
        int AddNewPart(Part part, bool? insertAtBottom, int? selectedRank);
        void SaveChangesPart(Part part, bool? insertAtBottom = null, int? selectedRank = null);
        IQueryable<Part> SearchParts(string searchString, Guid? categoryId);
    }
}