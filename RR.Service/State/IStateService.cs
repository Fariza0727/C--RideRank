using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.Service
{
     public interface IStateService : IDisposable
     {
          Task<IEnumerable<DropDownDto>> GetStates(int countryId);
          Task<DropDownDto> GetStateById(int? id);
          Task<StateDto> RestrictPlayer(string stateName, DateTime dob);
          Task<StateDto> GetStateByCode(string code);
          Task<int> GetCountryIdByStateCode(string code);
          Task<StateDto> GetStateByName(string name);
    }
}