using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country, RankRideContext> _repoCountry;

        public CountryService(IRepository<Country, RankRideContext> repoCountry)
        {
            _repoCountry = repoCountry;
        }

        public async Task<IEnumerable<DropDownDto>> GetCountries()
        {
            var countries = _repoCountry.Query().Get();
            return await Task.FromResult(CountryMapper.Map(countries));
        }

        public async Task<DropDownDto> GetCountryById(int? id)
        {
            var country = _repoCountry
                 .Query()
                 .Filter(x => x.Id == id)
                 .Get()
                 .SingleOrDefault();
            return await Task.FromResult(country != null ? CountryMapper.Map(country) : new DropDownDto());
        }
        public void Dispose()
        {
            if (_repoCountry != null)
            {
                _repoCountry.Dispose();
            }
        }

        public Task<DropDownDto> GetCountryByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var country_ = _repoCountry.Query().Filter(r => r.Name.ToLower().Trim() == name.ToLower().Trim()).Get().SingleOrDefault();
                if (country_ != null)
                {
                    return Task.FromResult(new DropDownDto { Name = country_.Name, Id = country_.Id });
                }
            }

            return Task.FromResult(default(DropDownDto));

        }
    }
}
