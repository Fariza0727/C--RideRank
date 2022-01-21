using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface ICityService : IDisposable
     {
          Task<IEnumerable<DropDownDto>> GetCitiesByCountryId(int countryId);

          Task<IEnumerable<DropDownDto>> GetCitiesByStateId(int stateId);

          Task<DropDownDto> GetCityById(int? id);
     }
}