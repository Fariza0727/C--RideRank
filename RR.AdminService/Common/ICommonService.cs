using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface ICommonService
     {
          Task<List<SelectListItem>> GetCountries();

          Task<string> GetCountryById(int? id);

          Task<List<SelectListItem>> GetCitiesByCountryId(int countryId);

          Task<List<SelectListItem>> GetCitiesByStateId(int id);

          Task<List<SelectListItem>> GetStates(int id);

          Task<string> GetStateById(int? id);

          Task<string> GetCityById(int? id);
     }
}