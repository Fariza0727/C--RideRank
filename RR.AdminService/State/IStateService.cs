using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface IStateService : IDisposable
     {
          Task<IEnumerable<DropDownDto>> GetStates(int countryId);
          Task<DropDownDto> GetStateById(int? id);
     }
}