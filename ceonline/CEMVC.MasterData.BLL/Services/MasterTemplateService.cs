using CEMVC.Core.DAL.Infrastructure;
using CEMVC.Core.DAL.RemodelMAX;
//using CEMVC.Core.DAL.IRepositories;
using CEMVC.Core.DAL.RemodelMAX.IRepositories;
using CEMVC.MasterData.BLL.Infrastructure;
using System;
using System.Linq;

namespace CEMVC.MasterData.BLL.Services
{
    public class MasterTemplateService
    {
        private readonly ITemplateRepository _templateRepository;
        private readonly ITemplate_CategoryRepository _categoryRepository;
        private readonly ITemplate_PartRepository _templatePartRepository;
        private readonly IPartRepository _partRepository;
        private readonly IUnitOfCEMasterWork _unitOfWork;

        public MasterTemplateService(ITemplateRepository templateRepository, ITemplate_CategoryRepository categoryRepository,
            ITemplate_PartRepository templatePartRepository, IPartRepository partRepository,
                                    IUnitOfCEMasterWork unitOfWork)
        {
            _templateRepository = templateRepository;
            _categoryRepository = categoryRepository;
            _templatePartRepository = templatePartRepository;
            _partRepository = partRepository;
            _unitOfWork = unitOfWork;
        }

        public Template GetById(Guid uid)
        {
            return _templateRepository.GetAll().FirstOrDefault(t => t.unique_id == uid);
        }

        public Template GetByIdEager(Guid uid)
        {
            return _templateRepository.GetAllEager().FirstOrDefault(t => t.unique_id == uid);
        }

        public IQueryable<Template> GetTemplatesByCategory(Guid category_uid)
        {
            return _templateRepository.GetAll().Where(t => t.category_uid == category_uid && t.deleted_at == null);
        }

        public IQueryable<Template_Category> GetAllTemplateCategories()
        {
            return _categoryRepository.GetAll().Where(t => t.deleted_at == null);
        }

        public IQueryable<Template> FollowUpTemplates()
        {
            return _templateRepository.GetAll().Where(t => (t.option_flags & (int)TemplateUseEnum.FollowUp) !=0 && t.deleted_at == null);
        }

        public void Create(Template template)
        {
            if (template.unique_id == Guid.Empty)
                template.unique_id = Guid.NewGuid();
            template.created_at = DateTime.UtcNow;

            _templateRepository.Add(template);
            _unitOfWork.Commit();
        }

        public Template Copy(Guid id)
        {
            var src = GetById(id);
            var newTemplate = new Template {
                category_uid = src.category_uid,
                created_at = DateTime.UtcNow,
                description = src.description,
                id = src.id,
                option_flags = src.option_flags,
                title = ((src.title ?? "").StartsWith("[Copy of]") ? "" : "[Copy of] ") + src.title,
                type_id = src.type_id,
                type_uid = src.type_uid,
                unique_id = Guid.NewGuid()
            };
            _templateRepository.Add(newTemplate);

            foreach (var p in src.TemplateParts.Where(t => t.deleted_at == null))
            {
                _templatePartRepository.Add(new Template_Part
                {
                    template_uid = newTemplate.unique_id,
                    created_at = DateTime.UtcNow,
                    part_uid = p.part_uid,
                    part_id = p.part_id,
                    qty_every = p.qty_every,
                    qty_fixed = p.qty_fixed,
                    qty_per = p.qty_per,
                    unique_id = Guid.NewGuid()
                });
            }
            
            _unitOfWork.Commit();
            return GetById(newTemplate.unique_id);
        }

        public void Save(Template template)
        {
            template.updated_at = DateTime.UtcNow;
            if((template.option_flags & (int)TemplateUseEnum.FollowUp) == 0 && template.Templates1.Any())
                foreach(var tmpl in _templateRepository.GetAll().Where(t => t.follow_up_uid == template.unique_id).ToList())
                {
                    tmpl.follow_up_uid = null;
                    _templateRepository.Update(tmpl);
                }

            _templateRepository.Update(template);
            _unitOfWork.Commit();
        }

        public void Remove(Guid uid)
        {
            var tmpl = GetById(uid);
            if (tmpl != null)
            {
                var time = DateTime.UtcNow;
                for (int i = 0; i < tmpl.TemplateParts.Count; i++)
                {
                    tmpl.TemplateParts.ElementAt(i).deleted_at = time;
                }
                tmpl.deleted_at = time;
                _templateRepository.Update(tmpl);
                _unitOfWork.Commit();
            }
        }

        public Guid AddPartToTemplate(Guid templateId, Guid partId, double? qty_per, double? qty_fixed, int? qty_every)
        {
            var tmpl = GetById(templateId);
            if (tmpl == null)
            {
                throw new InvalidOperationException("Template not found");
            }

            var tmplPartExisting = tmpl.TemplateParts.FirstOrDefault(p => p.part_uid == partId);
            if (tmplPartExisting != null)
            {
                tmplPartExisting.qty_per = qty_per;
                tmplPartExisting.qty_fixed = qty_fixed;
                tmplPartExisting.qty_every = qty_every;
                tmplPartExisting.deleted_at = null; //case when a part was deleted
                tmplPartExisting.updated_at = DateTime.UtcNow;
                _templatePartRepository.Update(tmplPartExisting);
                _unitOfWork.Commit();

                return tmplPartExisting.unique_id;
            }

            var part = _partRepository.GetAll().FirstOrDefault(p => p.unique_id == partId);
            if (part == null)
                throw new InvalidOperationException("Part not found");

            var tmplPartNew = new Template_Part
            {
                unique_id = Guid.NewGuid(),
                template_uid = templateId,
                part_uid = partId,
                qty_every = qty_every,
                qty_fixed = qty_fixed,
                qty_per = qty_per,
                created_at = DateTime.UtcNow
            };

            _templatePartRepository.Add(tmplPartNew);
            tmpl.updated_at = DateTime.UtcNow;
            _templateRepository.Update(tmpl);
            _unitOfWork.Commit();

            return tmplPartNew.unique_id;
        }

        public void RemovePartFromTemplate(Guid partId)
        {
            var templatePart = _templatePartRepository.GetAll().FirstOrDefault(p => p.unique_id == partId);
            if (templatePart != null)
            {
                templatePart.deleted_at = DateTime.UtcNow;
                _templatePartRepository.Update(templatePart);
                templatePart.Template.updated_at = DateTime.UtcNow;
                _templateRepository.Update(templatePart.Template);
                _unitOfWork.Commit();
            }
        }        

        public Template_Part GetTemplatePart(Guid id)
        {
            return _templatePartRepository.GetAll().FirstOrDefault(p => p.unique_id == id);
        }

        public void UpdateTemplatePart(Template_Part part)
        {
            var tmplPart = _templatePartRepository.GetAll().FirstOrDefault(p => p.unique_id == part.unique_id);
            tmplPart.updated_at = DateTime.UtcNow;
            _templatePartRepository.Update(part);
            if (tmplPart.Template != null)
            {
                tmplPart.Template.updated_at = DateTime.UtcNow;
                _templateRepository.Update(tmplPart.Template);
            }
            _unitOfWork.Commit();
        }
    }
}
