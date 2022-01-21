using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface ICountryService : IDisposable
     {
          Task<IEnumerable<DropDownDto>> GetCountries();

          Task<DropDownDto> GetCountryById(int? id);
     }
}