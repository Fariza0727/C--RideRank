using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace RR.Service
{
    public class CityService : ICityService
    {
        private readonly IRepository<City, RankRideContext> _repoCity;

        public CityService(IRepository<City, RankRideContext> repoCity)
        {
            _repoCity = repoCity;
        }

        public async Task<IEnumerable<DropDownDto>> GetCitiesByCountryId(int countryId)
        {
            var cities = _repoCity.Query().Filter(c => c.CountryId == countryId).Get();
            return await Task.FromResult(CityMapper.Map(cities));
        }

        public async Task<IEnumerable<DropDownDto>> GetCitiesByStateId(int stateId)
        {
            var cities = _repoCity.Query().Filter(c => c.StateId == stateId).Get();
            return await Task.FromResult(CityMapper.Map(cities));
        }

        public async Task<DropDownDto> GetCityById(int? id)
        {
            var city = _repoCity
                 .Query()
                 .Filter(x => x.Id == id)
                 .Get()
                 .SingleOrDefault();
            return await Task.FromResult(city != null ? CityMapper.Map(city) : new DropDownDto());
        }

        public void Dispose()
        {
            if (_repoCity != null)
            {
                _repoCity.Dispose();
            }
        }

      
    }
}
