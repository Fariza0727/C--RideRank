using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public static class UserRoleMapper
     {
          public static IEnumerable<DropDownDto> Map(IEnumerable<AspNetRoles> roles)
          {
               return roles.Select(p => Map(p));
          }

          public static DropDownDto Map(AspNetRoles role)
          {
               return new DropDownDto
               {
                    Id = role.Id,
                    Name = role.NormalizedName
               };
          }
     }
}
