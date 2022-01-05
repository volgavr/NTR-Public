using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CEMVC.Core.BLL.Exceptions;
using CEMVC.Core.BLL.Enums;
using CEMVC.Core.BLL.Infrastructure;
//using CEMVC.Core.DAL;
using CEMVC.Core.DAL.RemodelMAX.IRepositories;
using CEMVC.Core.DAL.Infrastructure;
using CEMVC.Core.DAL.RemodelMAX;

namespace CEMVC.MasterData.BLL.Services
{
    /// <summary>
    /// Service class for Part entites
    /// </summary>
    public class MasterPartService : IMasterPartService
    {
        private readonly IPartRepository _partRepository;
        private readonly IPart_TypeRepository _partTypeRepository;
        private readonly IUnitOfCEMasterWork _masterUnitOfWork;

        public MasterPartService(IPartRepository partRepository, IPart_TypeRepository partTypeRepository, IUnitOfCEMasterWork unitOfWork)
        {
            _partRepository = partRepository;
            _partTypeRepository = partTypeRepository;
            _masterUnitOfWork = unitOfWork;
        }

        /// <summary>
        /// Return list of all parts available for user
        /// </summary>
        /// <param name="includeDeleted"> if true thenk result will include mark as deleted parts</param>
        /// <returns></returns>
        public IQueryable<Part> GetAllParts(bool includeDeleted = false)
        {
            return includeDeleted ? _partRepository.GetAll() : _partRepository.GetAll().Where(x => x.deleted_at == null);
        }

        /// <summary>
        /// Return list of parts available for user with selected category
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<Part> GetPartsByCategoryId(Guid id, PartCategoryLevelEnum level = PartCategoryLevelEnum.MainCategory)
        {
            switch (level)
            {
                case PartCategoryLevelEnum.MainCategory:
                    return GetAllParts().Where(x => x.category_unique_id == id);
                case PartCategoryLevelEnum.SubCategory:
                    return GetAllParts().Where(x => x.subcategory_unique_id == id);
                case PartCategoryLevelEnum.Group:
                    return GetAllParts().Where(x => x.group_unique_id == id);
                default:
                    throw new ArgumentOutOfRangeException("level");
            }

        }        

        /// <summary>
        /// Return Part by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Obsolete]
        public Part GetById(int id)
        {
            return _partRepository.GetAll().FirstOrDefault(x => x.id == id);
        }

        public Part GetByUid(Guid uid)
        {
            return _partRepository.GetAll().FirstOrDefault(x => x.unique_id == uid);
        }

        public IQueryable<Part> GetPartsByCategoryIds(int category_id, int subcategory_id, int group_id)
        {
            return _partRepository.GetAll().Where(x => x.category_id == category_id && x.subcategory_id == subcategory_id && x.group_id == group_id);
        }

        public IQueryable<Part> GetPartsByCategoryIds(Guid category_uid, Guid subcategory_uid, Guid group_uid)
        {
            return _partRepository.GetAll().Where(x => x.category_unique_id == category_uid && x.subcategory_unique_id == subcategory_uid && x.group_unique_id == group_uid);
        }

        public IQueryable<Part> SearchParts(string searchString, Guid? category_uid)
        {
            if (string.IsNullOrEmpty(searchString))
                throw new ArgumentException("searchString");
            return category_uid.HasValue ? 
                _partRepository.GetAll().Where(x => x.category_unique_id == category_uid && x.title.Contains(searchString)) :
                _partRepository.GetAll().Where(x => x.title.Contains(searchString));
        }

        public void SaveChangesPart(Part part, bool? insertAtBottom = null, int? selectedRank = null)
        {
            if (_partRepository.GetAll().Any(x => x.part_code.Length > 0 && x.part_code == part.part_code && x.category_unique_id == part.category_unique_id && x.unique_id != part.unique_id))
            {
                throw new PartCodeAlreadyUsed(part.part_code);
            }
            //if (image != null)
            //{
            //    if (part.image_id.HasValue)
            //    {
            //        var imgId = part.image_id.Value;
            //        _imageStoreRepository.Delete(x => x.id == imgId);
            //    }
            //    _imageStoreRepository.Add(image);
            //    part.image_id = image.id;
            //}

            if(part.type_unique_id.HasValue)
            {
                part.type_id = _partTypeRepository.GetAll().FirstOrDefault(t => t.unique_id == part.type_unique_id.Value)?.id;
            }

            if (insertAtBottom.HasValue)
            {
                var maxRank = _partRepository.GetAll().Where(x => x.category_unique_id == part.category_unique_id).Max(x => x.rank); // && x.subcategory_unique_id == part.subcategory_unique_id && x.group_unique_id == part.group_unique_id
                if (insertAtBottom.Value)
                    part.rank = (maxRank ?? 0) + 1;
                else
                {
                    var list = _partRepository.GetAll().Where(x => x.category_unique_id == part.category_unique_id && x.subcategory_unique_id == part.subcategory_unique_id && x.group_unique_id == part.group_unique_id && x.rank >= selectedRank).ToList(); // && x.subcategory_unique_id == part.subcategory_unique_id && x.group_unique_id == part.group_unique_id
                    for (var i = 0; i < list.Count(); i++)
                    {
                        var elem = list.ElementAt(i);
                        elem.rank++;
                        _partRepository.Update(elem);
                    }
                    part.rank = selectedRank ?? (maxRank ?? 0) + 1;
                }
            }

            part.updated_at = DateTime.UtcNow;
            _partRepository.Update(part);
            _masterUnitOfWork.Commit();

        }

        public int AddNewPart(Part part, bool? insertAtBottom, int? selectedRank)
        {
            if (_partRepository.GetAll().Any(x => x.part_code.Length > 0 && x.category_unique_id == part.category_unique_id && x.part_code == part.part_code))
            {
                throw new PartCodeAlreadyUsed(part.part_code);
            }

            if (insertAtBottom.HasValue)
            {
                var maxRank = _partRepository.GetAll().Where(x => x.category_unique_id == part.category_unique_id).Max(x => x.rank); // && x.subcategory_unique_id == part.subcategory_unique_id && x.group_unique_id == part.group_unique_id
                if (insertAtBottom.Value)
                    part.rank = (maxRank ?? 0) + 1;
                else
                {
                    var list = _partRepository.GetAll().Where(x => x.category_unique_id == part.category_unique_id && x.rank >= selectedRank).ToList(); // && x.subcategory_unique_id == part.subcategory_unique_id && x.group_unique_id == part.group_unique_id
                    for (var i = 0; i < list.Count(); i++)
                    {
                        var elem = list.ElementAt(i);
                        elem.rank++;
                        _partRepository.Update(elem);
                    }
                    part.rank = selectedRank ?? (maxRank ?? 0) + 1;
                }
            }
            part.created_at = DateTime.UtcNow;
            _partRepository.Add(part);
            _masterUnitOfWork.Commit();

            return part.id;
        }


        [Obsolete]
        public void RemovePart(int id)
        {
            _partRepository.Delete(x => x.id == id);
            _masterUnitOfWork.Commit();
        }

        public void RemovePart(Guid uid)
        {
            var part = GetByUid(uid);
            if (part == null)
                return;
            part.deleted_at = DateTime.UtcNow;
            _partRepository.Update(part);
            _masterUnitOfWork.Commit();
        }
    }
}
