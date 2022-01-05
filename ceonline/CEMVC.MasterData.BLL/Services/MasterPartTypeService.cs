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
    public class MasterPartTypeService
    {        
        private readonly IPart_TypeRepository _partTypeRepository;
        private readonly IUnitOfCEMasterWork _masterUnitOfWork;
        private Dictionary<int, Part_Type> _partTypes;
        //private Dictionary<Guid, Part_Type> _partGuidTypes;

        public MasterPartTypeService(IPart_TypeRepository partTypeRepository, IUnitOfCEMasterWork unitOfWork)
        {
            _partTypeRepository = partTypeRepository;
            _masterUnitOfWork = unitOfWork;
        }

        public IEnumerable<Part_Type> GetAll()
        {
            if (_partTypes == null)
            {
                _partTypes = _partTypeRepository.GetAll().ToDictionary(t => t.id);

            }            

            return _partTypes.Values;
        }

        public Part_Type GetById(int id)
        {
            return _partTypes != null ? _partTypes[id] : _partTypeRepository.GetAll().FirstOrDefault(x => x.id == id);
        }

        public Part_Type GetByUid(Guid uid)
        {            
            return _partTypes != null ? _partTypes.Values.First(d => d.unique_id == uid) : _partTypeRepository.GetAll().FirstOrDefault(x => x.unique_id == uid);
        }
    }
}
