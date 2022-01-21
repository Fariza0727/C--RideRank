using RR.Data;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class CityMapper
     {
          /// <summary>
          /// Map List Of City To DropDownDto
          /// </summary>
          /// <param name="cities">List Of Cities</param>
          /// <returns>List Of DropDownDto</returns>
          public static IEnumerable<DropDownDto> Map(IEnumerable<City> cities)
          {
               return cities.Select(p => Map(p));
          }

          /// <summary>
          /// Map City To DropDownDto
          /// </summary>
          /// <param name="city">The City</param>
          /// <returns>The DropDownDto</returns>
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
