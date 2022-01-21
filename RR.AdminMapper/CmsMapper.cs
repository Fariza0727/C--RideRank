using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public class CmsMapper
     {
          /// <summary>
          /// Map Cms To CmsDto
          /// </summary>
          /// <param name="users"></param>
          /// <returns></returns>
          public static IEnumerable<CmsDto> Map(IEnumerable<Cms> cms)
          {
               return cms.Select(p => MapDto(p));
          }

          /// <summary>
          /// MapDto
          /// </summary>
          /// <param name="cms">The Cms</param>
          /// <returns>The CmsDto</returns>
          public static CmsDto MapDto(Cms cms)
          {
               return new CmsDto
               {
                    Id = cms.Id,
                    MetaDescription = cms.MetaDescription,
                    MetaKeyword = cms.MetaKeyword,
                    MetaTitle = cms.MetaTitle,
                    PageContent = cms.PageContent,
                    PageName = cms.PageName,
                    PageUrl = cms.PageUrl,
                    CreatedBy = cms.CreatedBy,
                    CreatedDate = cms.CreatedDate,
                    UpdatedBy = cms.UpdatedBy,
                    UpdatedDate = cms.UpdatedDate
               };
          }
     }
}
