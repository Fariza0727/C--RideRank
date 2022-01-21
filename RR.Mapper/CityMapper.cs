using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public static class CityMapper
     {
          public static IEnumerable<DropDownDto> Map(IEnumerable<City> cities)
          {
               return cities.Select(p => Map(p));
          }
          public static DropDownDto Map(City city)
          {
               return new DropDownDto
               {
                    Id = city.Id,
                    Name = city.Name
               };
          }
     }
}
