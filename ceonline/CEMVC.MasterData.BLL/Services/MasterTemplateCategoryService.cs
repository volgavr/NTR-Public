using CEMVC.Core.DAL.Infrastructure;
using CEMVC.Core.DAL.RemodelMAX;
using CEMVC.Core.DAL.RemodelMAX.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEMVC.MasterData.BLL.Services
{
    public class MasterTemplateCategoryService
    {
        private readonly ITemplate_CategoryRepository _categoryRepository;
        private readonly IUnitOfCEMasterWork _unitOfWork;

        public MasterTemplateCategoryService(ITemplate_CategoryRepository categoryRepository, IUnitOfCEMasterWork masterUnitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = masterUnitOfWork;
        }

        public IQueryable<Template_Category> GetAll(bool include_deleted = false)
        {
            return include_deleted ?
           _categoryRepository.GetAll() : _categoryRepository.GetAll().Where(c => c.deleted_at == null);
        }

        public Template_Category GetById(Guid uid)
        {
            return _categoryRepository.GetAll().FirstOrDefault(c => c.unique_id == uid);
        }

        public Guid Create(string title, bool? insertLast, int? insertPriorToRank)
        {
            var category = new Template_Category {
                created_at = DateTime.UtcNow,
                title = title,
                rank = 0,
                unique_id = Guid.NewGuid()
            };

            if (insertLast == true)
            {
                int? maxRank = _categoryRepository.GetAll().Max(x => x.rank);
                category.rank = (maxRank ?? 0) + 1;

                _categoryRepository.Add(category);
            }
            else
            {
                if (insertPriorToRank == null)
                    insertPriorToRank = _categoryRepository.GetAll().Min(x => x.rank) ?? 1;

                var tailToUpdate = _categoryRepository.GetAll().Where(x => x.rank >= insertPriorToRank).OrderBy(x => x.rank).ToList();
                for (var i = 0; i < tailToUpdate.Count(); i++)
                {
                    var elem = tailToUpdate.ElementAt(i);
                    elem.rank++;
                    _categoryRepository.Update(elem);
                }
                
                category.rank = insertPriorToRank;
                _categoryRepository.Add(category);
            }

            _unitOfWork.Commit();
            return category.unique_id;
        }

        public void Save(Template_Category category)
        {
            category.updated_at = DateTime.UtcNow;
            _categoryRepository.Update(category);
            _unitOfWork.Commit();
        }

        public void Remove(Guid uid)
        {
            var category = GetById(uid);
            if (category != null)
            {
                category.deleted_at = DateTime.UtcNow;
                category.rank = null;
                _categoryRepository.Update(category);
                _unitOfWork.Commit();
            }
        }
    }
}
