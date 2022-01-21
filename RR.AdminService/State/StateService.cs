using RR.AdminMapper;
using RR.Data;
using RR.Dto;
using RR.Repo;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
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
               return await Task.FromResult(StateMapper.Map(state));
          }

          public void Dispose()
          {
               if (_repoState != null)
               {
                    _repoState.Dispose();
               }
          }
     }
}
