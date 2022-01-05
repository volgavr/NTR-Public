using System;
using System.Collections.Generic;
using System.Linq;
using CEMVC.Core.BLL.Infrastructure;
using CEMVC.Core.DAL.IRepositories;
using CEMVC.Core.DAL.Infrastructure;
using CEMVC.Core.DAL.RemodelMAX.IRepositories;
using System.Data;
using CEMVC.Core.DAL.RemodelMAX;

using Version = CEMVC.Core.DAL.RemodelMAX.Version;
using Part = CEMVC.Core.DAL.RemodelMAX.Part;
using Location = CEMVC.Core.DAL.RemodelMAX.Location;

namespace CEMVC.MasterData.BLL.Services
{
    public class MasterPriceUpdateService
    {
        private readonly IUser_Price_Update_UrlsRepository _user_Price_Update_UrlsRepository;
        private readonly IPrice_Update_CashRepository _price_Update_CashRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IVersionRepository _versionRepository;
        private readonly CEMVC.Core.DAL.RemodelMAX.IRepositories.IPartRepository _partRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitOfCEMasterWork _masterUnitOfWork;
        private readonly ILogger _logger;

        //public class TinyPriceUpdate
        //{
        //    public string supplier_code{get;set;}
        //    public decimal price{get;set;}
        //}

        //public class WidePriceUpdate :TinyPriceUpdate
        //{
        //    public double? material_multiplier{get;set;}
        //}

        public MasterPriceUpdateService(IUser_Price_Update_UrlsRepository User_Price_Update_UrlsRepository,
                                    IVersionRepository versionRepository,
                                    IPrice_Update_CashRepository Price_Update_CashRepository,
                                    IPriceRepository PriceRepository,
                                    ILocationRepository locationRepository,
                                    //IVersionRepository versionRepository,
                                    Core.DAL.RemodelMAX.IRepositories.IPartRepository PartRepository,
                                    IUnitOfWork unitOfWork,
                                    IUnitOfCEMasterWork masterUnitOfWork,
                                    ILogger logger)
        {
            _user_Price_Update_UrlsRepository = User_Price_Update_UrlsRepository;
            //_price_Update_UrlsRepository = Price_Update_UrlsRepository;
            //_user_UrlsRepository = User_UrlsRepository;
            _versionRepository = versionRepository;
            _price_Update_CashRepository = Price_Update_CashRepository;
            _priceRepository = PriceRepository;
            _locationRepository = locationRepository;
            _partRepository = PartRepository;
            _unitOfWork = unitOfWork;
            _masterUnitOfWork = masterUnitOfWork;
            _logger = logger;
        }

        public IEnumerable<LocationVersion> GetLocationsAndVersionsInfo()
        {
            return GetLocations().GroupJoin(GetAllVersions(), x => x.unique_id, y => y.location_unique_id, (x, collection) =>
            new LocationVersion
            {
                Location = new Core.BLL.Infrastructure.Location() {
                        id = x.id,
                        unique_id = x.unique_id,
                        state = x.state,
                        city = x.city,
                        geolocation = x.geolocation,
                        lat = x.lat,
                        lon = x.lon,
                        url = x.url,
                        last_modified = x.last_modified },
                VersionDate = collection.OrderByDescending(i => i.created_at).FirstOrDefault()?.created_at
            });
        }

        public IEnumerable<Location> GetLocations()
        {
            return _locationRepository.GetAll();
        }

        public IQueryable<Version> GetAllVersions()
        {
            return _versionRepository.GetAll();
        }

        [Obsolete]
        public Location GetLocationById(long location_id)
        {
            return _locationRepository.GetById(location_id);
        }

        public Location GetLocationByUid(Guid location_uid)
        {
            return _locationRepository.GetAll().FirstOrDefault(l=> l.unique_id == location_uid);
        }

        public int AddPriceVersion(string title, Guid locationId, bool isEnabled = true)
        {
            var version = new Version
            {
                title = title,
                created_at = DateTime.UtcNow,
                location_unique_id = locationId,
                is_enabled = isEnabled
            };
            _versionRepository.Add(version);
            _masterUnitOfWork.Commit();
            return version.id;
        }

        public List<Part> GetPartsByPartCode(string part_code)
        {
            return _partRepository.GetAll().Where(x => x.part_code == part_code).ToList();
        }

        public void UpdatePart(Part part)
        {
            _partRepository.Update(part);
            _masterUnitOfWork.Commit();
        }

        public void AddPrice(Price price)
        {
            _priceRepository.Add(price);
            _masterUnitOfWork.Commit();
        }

        public void AddPrices(List<Price> prices)
        {
            try
            {
                _masterUnitOfWork.SetUp(new UnitOfWorkConfiguration { AutoDetectChangesEnabled = false, ValidateOnSaveEnabled = false });
                foreach(var price in prices)
                {
                    _priceRepository.Add(price);
                }
                _masterUnitOfWork.Commit();
            }
            catch(Exception e)
            {
                _logger.Error("An error occured while adding price updates", e);
            }
            finally
            {
                _masterUnitOfWork.Reset();
            }
        }

        public Location GetSampleLocation()
        {
            var query = _locationRepository.GetAll().Where(t => t.Prices.Any() && t.Labor_Rates.Any()).OrderBy(x => x.city);
            return query.FirstOrDefault();
        }
    }
}
