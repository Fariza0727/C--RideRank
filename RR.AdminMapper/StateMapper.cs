using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class StateMapper
     {
          /// <summary>
          /// Map List of State To DropDownDto
          /// </summary>
          /// <param name="states">List Of States</param>
          /// <returns>List Of DropDownDto</returns>
          public static IEnumerable<DropDownDto> Map(IEnumerable<State> states)
          {
               return states.Select(p => Map(p));
          }

          /// <summary>
          /// Map State To DropDownDto
          /// </summary>
          /// <param name="State">The State</param>
          /// <returns>The DropDownDto</returns>
          public static DropDownDto Map(State State)
          {
               return new DropDownDto
               {
                    Id = State.Id,
                    Name = State.Name
               };
          }
     }
}
