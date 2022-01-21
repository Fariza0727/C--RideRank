using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class ContestCategoryMapper
     {
          public static IEnumerable<DropDownDto> Map(IEnumerable<ContestCategory> contestCategories)
          {
               return contestCategories.Select(p => Map(p));
          }

          public static DropDownDto Map(ContestCategory contestcategory)
          {
               return new DropDownDto
               {
                    Id = contestcategory.Id,
                    Name = contestcategory.CategoryName
               };
          }
     }
}
