using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public static class CountryMapper
     {
          public static IEnumerable<DropDownDto> Map(IEnumerable<Country> countries)
          {
               return countries.Select(p => Map(p));
          }

          public static DropDownDto Map(Country country)
          {
               return new DropDownDto
               {
                    Id = country.Id,
                    Name = country.Name
               };
          }
     }
}
