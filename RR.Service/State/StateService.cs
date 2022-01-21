using Microsoft.EntityFrameworkCore;
using RR.Data;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service
{
    public class StateService : IStateService
    {
        private readonly IRepository<State, RankRideContext> _repoState;

        public StateService(IRepository<State, RankRideContext> repoState)
        {
            _repoState = repoState;
        }

        public async Task<IEnumerable<DropDownDto>> GetStates(int countryId)
        {
            var states = _repoState.Query().Filter(c => c.CountryId == countryId).Get();
            return await Task.FromResult(StateMapper.Map(states));
        }

        public async Task<DropDownDto> GetStateById(int? id)
        {
            var state = _repoState
                 .Query()
                 .Filter(x => x.Id == id)
                 .Get()
                 .SingleOrDefault();
            return await Task.FromResult(state != null ? StateMapper.Map(state) : new DropDownDto());
        }

        public async Task<StateDto> RestrictPlayer(string stateName, DateTime dob)
        {
            stateName = stateName.Replace(" ", "").ToLower();

            // Save today's date.
            var today = DateTime.Today;
            // Calculate the age.
            var age = today.Year - dob.Year;
            // Go back to the year the person was born in case of a leap year
            if (dob.Date > today.AddYears(-age)) age--;

            var state = _repoState
              .Query()
              .Filter(x => x.Name.Replace(" ", "").ToLower() == stateName || x.StateCode.ToLower() == stateName)
              .Includes(x => x.Include(y => y.Country))
              .Get()
              .SingleOrDefault();
            if (state != null)
            {
                if (state.Age <= age)
                {
                    return await Task.FromResult(StateMapper.MapDto((state)));
                }
                else
                {
                    return await Task.FromResult(new StateDto() { ErrorMessage = "The Age is not valid for this user!!" });
                }
            }
            else
            {
                return await Task.FromResult(new StateDto() { ErrorMessage = "Please select valid state!!" });
            }
        }

        public async Task<StateDto> GetStateByCode(string code)
        {
            var state = _repoState
              .Query()
              .Filter(x => x.StateCode.ToLower() == code.Trim().ToLower())
              .Includes(x => x.Include(y => y.Country))
              .Get()
              .SingleOrDefault();
            if (state != null)
            {

                return await Task.FromResult(StateMapper.MapDto((state)));
            }
            else
            {
                return await Task.FromResult(new StateDto() { ErrorMessage = "Please select valid state!!" });
            }
        }

        public async Task<int> GetCountryIdByStateCode(string code)
        {
            var state = _repoState
              .Query()
              .Filter(x => x.StateCode.ToLower() == code.Trim().ToLower())
              .Includes(x => x.Include(y => y.Country))
              .Get()
              .SingleOrDefault();
            if (state != null)
            {

                return await Task.FromResult(state.CountryId);
            }
            else
            {
                return await Task.FromResult(0);
            }
        }
        public void Dispose()
        {
            if (_repoState != null)
            {
                _repoState.Dispose();
            }
        }

        public Task<StateDto> GetStateByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var state_ = _repoState.Query().Filter(r => r.Name.ToLower().Trim() == name.ToLower().Trim()).Get().SingleOrDefault();
                if (state_ != null)
                {
                    return Task.FromResult(StateMapper.MapDto(state_));
                }
            }

            return Task.FromResult(default(StateDto));
        }
    }
}
