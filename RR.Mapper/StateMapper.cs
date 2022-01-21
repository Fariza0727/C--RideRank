using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
    public static class StateMapper
    {
        public static IEnumerable<DropDownDto> Map(IEnumerable<State> states)
        {
            return states.Select(p => Map(p));
        }
        public static DropDownDto Map(State state)
        {
            return new DropDownDto
            {
                Id = state.Id,
                Name = state.Name
            };
        }

        public static StateDto MapDto(State state)
        {
            return new StateDto
            {
                IsIntermediate = state.IsIntermediate,
                IsNovice = state.IsNovice,
                IsPro = state.IsPro,
                Name = state.Name,
                StateCode = state.StateCode,
                CountryId = state.CountryId,
                CountryName = state.Country.Name,
                StateId = state.Id
            };
        }
    }
}
