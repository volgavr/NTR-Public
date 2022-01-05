using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CEMVC.Core.BLL.Exceptions;
using CEMVC.Core.BLL.Infrastructure;
using CEMVC.Core.DAL.RemodelMAX;
using CEMVC.Core.DAL.RemodelMAX.IRepositories;
using CEMVC.Core.DAL.Infrastructure;

namespace CEMVC.MasterData.BLL.Services
{
    /// <summary>
    /// Service class for labor_rate entites
    /// </summary>
    public class MasterLaborRateService
    {
        private readonly ILabor_RateRepository _laborRatesRepository;
        private readonly IUnitOfCEMasterWork _unitOfWork;
        public MasterLaborRateService(ILabor_RateRepository laborRatesRepository, IUnitOfCEMasterWork unitOfWork)            
        {
            _laborRatesRepository = laborRatesRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Return list of all parts available for user
        /// </summary>
        /// <returns></returns>
        public IQueryable<Labor_Rate> GetAllLaborRates(Guid? location_uid = null)
        {
            var query = _laborRatesRepository.GetAll();
            if(location_uid != null)
                query = query.Where(x => x.location_unique_id == location_uid);
            return query;
        }

        /// <summary>
        /// Return labor_Rates by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Labor_Rate GetById(int id)
        {
            var rate = _laborRatesRepository.GetAll().FirstOrDefault(x => x.id == id);
            //CheckAccessSecurity(rate);
            return rate;
        }

        /// <summary>
        /// Return labor_Rate by Letter
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public Labor_Rate GetByLetter(char letter)
        {
            var tempLetter = letter.ToString();
            var rate = _laborRatesRepository.Get(x => x.letter == tempLetter);
            //if (rate!=null)
            //CheckAccessSecurity(rate);
            return rate;
        }

        /// <summary>
        /// Return labor_Rate by Letter
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public Labor_Rate GetByLetter(String letter)
        {
            var rate = _laborRatesRepository.Get(x => x.letter == letter.Substring(0, 1));
            //if (rate!=null)
            //CheckAccessSecurity(rate);
            return rate;
        }

        /// <summary>
        /// Add new Labor_Rate in the DB
        /// </summary>
        /// <param name="rate"></param>
        public void AddNewLaborRate(Labor_Rate rate)
        {

            //rate.user_id = _userId;
            _laborRatesRepository.Add(rate);
            _unitOfWork.Commit();

        }

        /// <summary>
        /// Remove laborRate from DB 
        /// </summary>
        /// <param name="id"></param>
        public void RemoveLaborRate(int id)
        {
            var rateToDelete = _laborRatesRepository.GetById(id);
            //CheckAccessSecurity(rateToDelete);
            _laborRatesRepository.Delete(x => x.id == id);
            _unitOfWork.Commit();
        }


        /// <summary>
        /// Update existing entity in the DB
        /// </summary>
        /// <param name="laborRate"></param>
        public void SaveChangesLaborRate(Labor_Rate laborRate)
        {
            //CheckAccessSecurity(laborRate);
            _laborRatesRepository.Update(laborRate);
            _unitOfWork.Commit();

        }

    }
}
