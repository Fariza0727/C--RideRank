using RR.AdminData;
using RR.Dto;
using System.Collections.Generic;
using System.Linq;

namespace RR.Mapper
{
     public static class CMSMapper
     {
          /// <summary>
          /// Map Cms to CmsDto
          /// </summary>
          /// <param name="cms">List of Cms</param>
          /// <returns>List Of CmsDto</returns>
          public static IEnumerable<CmsDto> Map(IEnumerable<Cms> cms)
          {
               return cms.Select(p => MapDto(p));
          }

          /// <summary>
          /// Map Dto
          /// </summary>
          /// <param name="cmsDto">The Cms</param>
          /// <returns>The CmsDto</returns>
          public static CmsDto MapDto(Cms cms)
          {
            if (cms == null)
                return null;

               return new CmsDto
               {
                    Id = cms.Id,
                    CreatedBy = cms.CreatedBy,
                    CreatedDate = cms.CreatedDate,
                    MetaDescription = cms.MetaDescription,
                    MetaKeyword = cms.MetaKeyword,
                    MetaTitle = cms.MetaTitle,
                    PageContent = cms.PageContent,
                    PageName = cms.PageName,
                    PageUrl = cms.PageUrl,
                    UpdatedBy = cms.UpdatedBy,
                    UpdatedDate = cms.UpdatedDate
               };
          }
     }
}
