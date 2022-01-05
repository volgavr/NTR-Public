using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting;
using CEMVC.Core.BLL.Exceptions;
using CEMVC.Core.DAL.RemodelMAX;
using CEMVC.Core.DAL.RemodelMAX.IRepositories;
using CEMVC.Core.BLL.Infrastructure;
using CEMVC.Core.DAL.Infrastructure;
using CEMVC.Core.BLL.Enums;

namespace CEMVC.MasterData.BLL.Services
{
    /// <summary>
    ///     Service class for Part_Catogory entites
    /// </summary>
    public class MasterPartCategoryService : IMasterPartCategoryService
    {
        private readonly IPart_CategoryRepository _partCategoriesRepository;
        private readonly IPartRepository _partRepository;
        private readonly IUnitOfCEMasterWork _masterUnitOfWork;

        public MasterPartCategoryService(IPart_CategoryRepository partCategoriesRepository, IPartRepository partRepository, IUnitOfCEMasterWork masterUnitOfWork)
        {
            _partCategoriesRepository = partCategoriesRepository;
            _partRepository = partRepository;
            _masterUnitOfWork = masterUnitOfWork;
        }

        public int AddNewDefaultPartCategory(Part_Category category, bool? insertABottom)
        {
            if (category.parent_unique_id != null || category.parent_id != null)
                throw new ArgumentOutOfRangeException("Main category can not have a parent.");

            var result = AddNewDefaultPartCategory(category, insertABottom, category.rank);
            var misc_subcategory = new Part_Category()
            {
                title = RemodelMAXConstants.SUBCATEGORY_TITLE,
                unique_id = Guid.NewGuid(),
                parent_id = result,
                parent_unique_id = category.unique_id,
                rank = 0,
                is_default = true,
            };
            AddNewDefaultPartSubCategory(misc_subcategory, true);
            return result;
        }

        public int AddNewDefaultPartSubCategory(Part_Category subcategory, bool? insertABottom)
        {
            if (subcategory.parent_unique_id == null && subcategory.parent_id == null)
                throw new ArgumentOutOfRangeException("Subcategory must have a parent.");

            var result = AddNewDefaultPartCategory(subcategory, insertABottom, subcategory.rank);
            var misc_group = new Part_Category()
            {
                title = RemodelMAXConstants.GROUP_TITLE,
                unique_id = Guid.NewGuid(),
                parent_unique_id = subcategory.unique_id,
                parent_id = result,
                rank = 0,
                is_default = true,
            };
            AddNewDefaultPartGroup(misc_group, true);
            return result;
        }

        public int AddNewDefaultPartGroup(Part_Category group, bool? insertABottom)
        {
            if (group.parent_unique_id == null && group.parent_id == null)
                throw new ArgumentOutOfRangeException("Group must have a parent.");

            return AddNewDefaultPartCategory(group, insertABottom, group.rank);
        }

        private int AddNewDefaultPartCategory(Part_Category category, bool? insertABottom, int? selectedRank)
        {
            category.created_at = DateTime.UtcNow;

            if (!insertABottom.HasValue)
            {
                _partCategoriesRepository.Add(category);
            }
            else
            {
                if (insertABottom.Value)
                {
                    int? maxRank = _partCategoriesRepository.GetAll().Where(x => x.parent_unique_id == category.parent_unique_id).Max(x => x.rank);
                    category.rank = (maxRank ?? 0) + 1;

                    _partCategoriesRepository.Add(category);
                }
                else
                {
                    var list =
                        _partCategoriesRepository.GetAll().Where(x => x.parent_unique_id == category.parent_unique_id)
                                                 .Where(x => x.rank >= selectedRank)
                                                 .ToList();
                    for (var i = 0; i < list.Count(); i++)
                    {
                        var elem = list.ElementAt(i);
                        elem.rank++;
                        _partCategoriesRepository.Update(elem);
                    }
                    category.rank = selectedRank ?? ((_partCategoriesRepository.GetAll().Where(x => x.parent_unique_id == category.parent_unique_id).Max(x => x.rank) ?? 0) + 1);
                    _partCategoriesRepository.Add(category);
                }
            }
            _masterUnitOfWork.Commit();
            return category.id;
        }

        public IQueryable<Part_Category> GetAllCategories()
        {
            return _partCategoriesRepository.GetAll();
        }

        public IQueryable<Part_Category> GetAllDefaultCategories()
        {
            return _partCategoriesRepository.GetAll().Where(c => c.parent_unique_id == null && c.deleted_at == null);
        }

        [Obsolete]
        public Part_Category GetById(int id)
        {
            Part_Category partCategory = _partCategoriesRepository.GetById(id);
            return partCategory;
        }

        public Part_Category GetByUid(Guid uid)
        {
            Part_Category partCategory = _partCategoriesRepository.GetAll().FirstOrDefault(c => c.unique_id == uid);
            return partCategory;
        }

        [Obsolete]
        public IQueryable<Part_Category> GetChildCategories(int? parentId)
        {
            if (parentId.HasValue)
            {
                return _partCategoriesRepository.GetAll().Where(x => x.parent_id == parentId);
            }
            else
            {
                return _partCategoriesRepository.GetAll().Where(x => x.parent_id == null);
            }
        }

        public IQueryable<Part_Category> GetChildCategories(Guid? parentUid)
        {
            if (parentUid.HasValue)
            {
                return _partCategoriesRepository.GetAll().Where(x => x.parent_unique_id == parentUid);
            }
            else
            {
                return _partCategoriesRepository.GetAll().Where(x => x.parent_unique_id == null);
            }
        }

        [Obsolete]
        public Part_Category GetChildDefaultCategory(int parentId)
        {
            return _partCategoriesRepository.GetAll().Where(x => x.parent_id == parentId && x.is_default).FirstOrDefault();
        }

        public Part_Category GetChildDefaultCategory(Guid parentUid)
        {
            return _partCategoriesRepository.GetAll().Where(x => x.parent_unique_id == parentUid && x.is_default).FirstOrDefault();
        }

        public void SaveChangesPartCategory(Part_Category category)
        {
            category.updated_at = DateTime.UtcNow;
            _partCategoriesRepository.Update(category);
            _masterUnitOfWork.Commit();
        }

        [Obsolete]
        public void UpdateParts(int category_id, int? parent_category_id, PartCategoryLevelEnum level)
        {
            var parentCategory = _partCategoriesRepository.GetAll().FirstOrDefault(x => x.id == parent_category_id);
            if (level == PartCategoryLevelEnum.SubCategory) {
                var parts = _partRepository.GetAll().Where(x => x.subcategory_id == category_id).ToList();
                foreach (var part in parts)
                {
                    part.category_id = parent_category_id;
                    part.category_unique_id = parentCategory.unique_id;
                    part.updated_at = DateTime.UtcNow;
                    _partRepository.Update(part);
                }
                _masterUnitOfWork.Commit();
            }
            else if (level == PartCategoryLevelEnum.Group)
            {
                var parts = _partRepository.GetAll().Where(x => x.group_id == category_id).ToList();
                foreach (var part in parts)
                {
                    part.subcategory_id = parent_category_id;
                    part.subcategory_unique_id = parentCategory.unique_id;
                    part.category_id = parentCategory.parent_id;
                    part.category_unique_id = parentCategory.parent_unique_id;
                    part.updated_at = DateTime.UtcNow;
                    _partRepository.Update(part);
                }
                _masterUnitOfWork.Commit();
            }
        }

        [Obsolete]
        public void MoveCategory(int id, int? parent_id, int direction, int rank)
        {
            var childCategories = GetChildCategories(parent_id).OrderBy(x => x.rank).ToList();
            var fromSwap = GetById(id);
            Part_Category toSwap;
            if (direction == 1)
            {
                toSwap = childCategories.Where(x => x.rank > rank).OrderBy(x => x.rank).FirstOrDefault();
            }
            else
            {
                toSwap = childCategories.Where(x => x.rank < rank).OrderBy(x => x.rank).LastOrDefault();
            }
            if (toSwap != null)
            {
                fromSwap.rank = toSwap.rank;
                fromSwap.updated_at = DateTime.UtcNow;
                toSwap.rank = rank;
                toSwap.updated_at = DateTime.UtcNow;
                SaveChangesPartCategory(toSwap);
                SaveChangesPartCategory(fromSwap);
            }
        }

        public void UpdateParts(Guid category_uid, Guid? parent_category_uid, PartCategoryLevelEnum level)
        {
            var parentCategory = _partCategoriesRepository.GetAll().FirstOrDefault(x => x.unique_id == parent_category_uid);
            if (level == PartCategoryLevelEnum.SubCategory) {                                   
                var parts = _partRepository.GetAll().Where(x => x.subcategory_unique_id == category_uid).ToList();
                foreach (var part in parts)
                {
                    part.category_unique_id = parent_category_uid;
                    part.category_id = parentCategory.id;
                    part.updated_at = DateTime.UtcNow;
                    _partRepository.Update(part);
                }
                _masterUnitOfWork.Commit();
            }
            else if (level == PartCategoryLevelEnum.Group)
            {
                var parts = _partRepository.GetAll().Where(x => x.group_unique_id == category_uid).ToList();
                foreach (var part in parts)
                {
                    part.subcategory_unique_id = parent_category_uid;
                    part.subcategory_id = parentCategory.id;
                    part.category_id = parentCategory.parent_id;
                    part.category_unique_id = parentCategory.parent_unique_id;
                    part.updated_at = DateTime.UtcNow;
                    _partRepository.Update(part);
                }
                _masterUnitOfWork.Commit();
            }
        }

        public void MoveCategory(Guid uid, Guid? parent_uid, int direction, int rank)
        {
            var childCategories = GetChildCategories(parent_uid).OrderBy(x => x.rank).ToList();
            var fromSwap = GetByUid(uid);
            Part_Category toSwap;
            if (direction == 1)
            {
                toSwap = childCategories.Where(x => x.rank > rank).OrderBy(x => x.rank).FirstOrDefault();
            }
            else
            {
                toSwap = childCategories.Where(x => x.rank < rank).OrderBy(x => x.rank).LastOrDefault();
            }
            if (toSwap != null)
            {
                fromSwap.rank = toSwap.rank;
                fromSwap.updated_at = DateTime.UtcNow;
                toSwap.rank = rank;
                toSwap.updated_at = DateTime.UtcNow;
                SaveChangesPartCategory(toSwap);
                SaveChangesPartCategory(fromSwap);
            }
        }
    }

}
