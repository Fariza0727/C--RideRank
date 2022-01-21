using RR.AdminMapper;
using RR.Data;
using RR.Dto;
using RR.Repo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
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
               return await Task.FromResult(CountryMapper.Map(country));
          }
          public void Dispose()
          {
               if (_repoCountry != null)
               {
                    _repoCountry.Dispose();
               }
          }
     }
}
