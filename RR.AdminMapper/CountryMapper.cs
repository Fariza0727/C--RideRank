using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class CountryMapper
     {
          /// <summary>
          /// Map List of countries to DropDownDto
          /// </summary>
          /// <param name="countries">List Of Countries</param>
          /// <returns>List Of DropDownDto</returns>
          public static IEnumerable<DropDownDto> Map(IEnumerable<Country> countries)
          {
               return countries.Select(p => Map(p));
          }

          /// <summary>
          /// Map Country To DropDownDto
          /// </summary>
          /// <param name="country">The Country</param>
          /// <returns>The DropDownDto</returns>
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
