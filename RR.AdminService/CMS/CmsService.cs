using RR.AdminData;
using RR.AdminMapper;
using RR.Core;
using RR.Dto;
using RR.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public class CmsService : ICmsService
     {
          #region Constructor
          private readonly IRepository<Cms, RankRideAdminContext> _repoCms;

          public CmsService(IRepository<Cms, RankRideAdminContext> repoCms)
          {
               _repoCms = repoCms;
          }

          #endregion

          /// <summary>
          /// Add Edit Cms
          /// </summary>
          /// <param name="cmsDto">The CmsDto</param>
          /// <returns></returns>
          public async Task AddEditCms(CmsDto cmsDto, string userId)
          {
               Cms cmsEntity = new Cms();
               if (cmsDto.Id > 0)
               {
                    cmsEntity = _repoCms.Query().Filter(x => x.Id == cmsDto.Id).Get().FirstOrDefault();
                    cmsEntity.UpdatedDate = DateTime.Now;
                    cmsEntity.UpdatedBy = userId;
               }
               else
               {
                    cmsEntity.CreatedDate = DateTime.Now;
                    cmsEntity.CreatedBy = userId;
                    cmsEntity.UpdatedDate = DateTime.Now;
                    cmsEntity.UpdatedBy = userId;
               }

               cmsEntity.MetaDescription = cmsDto.MetaDescription;
               cmsEntity.MetaKeyword = cmsDto.MetaKeyword;
               cmsEntity.MetaTitle = cmsDto.MetaTitle;
               cmsEntity.PageContent = cmsDto.PageContent;
               cmsEntity.PageName = cmsDto.PageName;
               cmsEntity.PageUrl = cmsDto.PageUrl;

               if (cmsDto.Id > 0)
               {
                    await _repoCms.UpdateAsync(cmsEntity);
               }
               else
               {
                    await _repoCms.InsertGraphAsync(cmsEntity);
               }
          }

          /// <summary>
          /// Get All Cms
          /// </summary>
          /// <param name="start">page Number</param>
          /// <param name="length">Number of Records On Page</param>
          /// <param name="searchStr">Search String</param>
          /// <param name="sort">Order</param>
          /// <returns>List of Cms Record</returns>
          public async Task<Tuple<IEnumerable<CmsLiteDto>, int>> GetAllCms(int start, int length, int column, string searchStr = "", string sort = "")
          {
               int count = 0;

               var predicate = PredicateBuilder.True<Cms>()
              .And(x => (searchStr == "" || x.MetaDescription.Contains(searchStr.ToLower())
              || x.MetaKeyword.Contains(searchStr.ToLower())
              || x.MetaTitle.Contains(searchStr.ToLower())
              || x.PageName.Contains(searchStr.ToLower())));

               var cms = _repoCms
                   .Query()
                   .Filter(predicate);

               if (FilterSortingVariable.CMS_PAGENAME == column)
               {
                    cms = (sort == "desc" ? cms.OrderBy(x => x.OrderByDescending(xx => xx.PageName)) : cms.OrderBy(x => x.OrderBy(xx => xx.PageName)));
               }
               if (FilterSortingVariable.CMS_PAGEURL == column)
               {
                    cms = (sort == "desc" ? cms.OrderBy(x => x.OrderByDescending(xx => xx.PageUrl)) : cms.OrderBy(x => x.OrderBy(xx => xx.PageUrl)));
               }
               if (FilterSortingVariable.CMS_METATITLE == column)
               {
                    cms = (sort == "desc" ? cms.OrderBy(x => x.OrderByDescending(xx => xx.MetaTitle)) : cms.OrderBy(x => x.OrderBy(xx => xx.MetaTitle)));
               }
               if (FilterSortingVariable.CMS_METAKEYWORD == column)
               {
                    cms = (sort == "desc" ? cms.OrderBy(x => x.OrderByDescending(xx => xx.MetaKeyword)) : cms.OrderBy(x => x.OrderBy(xx => xx.MetaKeyword)));
               }
               if (FilterSortingVariable.CMS_METADESCRIPTION == column)
               {
                    cms = (sort == "desc" ? cms.OrderBy(x => x.OrderByDescending(xx => xx.MetaDescription)) : cms.OrderBy(x => x.OrderBy(xx => xx.MetaDescription)));
               }

               return await Task.FromResult(new Tuple<IEnumerable<CmsLiteDto>, int>(cms
                       .GetPage(start, length, out count).Select(y => new CmsLiteDto
                       {
                            Id = y.Id,
                            PageName = y.PageName,
                            PageUrl = y.PageUrl,
                            MetaDescription = y.MetaDescription,
                            MetaKeyword = y.MetaKeyword,
                            MetaTitle = y.MetaTitle
                       }), count));
          }

          //public async Task<CmsDto> GetAllCmsRecords()
          //{
          //     var cmsData = _repoCms.Query().Get().FirstOrDefault();

          //     return await Task.FromResult(CmsMapper.MapDto(cmsData));
          //}

          /// <summary>
          /// Get Cms By Id
          /// </summary>
          /// <param name="id">Cms Id</param>
          /// <returns>The CmsDto</returns>
          public async Task<CmsDto> GetCmsById(int id)
          {
               var cmsData = _repoCms.Query().Filter(x => x.Id == id).Get().FirstOrDefault();

               return await Task.FromResult(CmsMapper.MapDto(cmsData));
          }

          /// <summary>
          /// Delete Cms By Id
          /// </summary>
          /// <param name="id">Cms Id</param>
          public async Task DeleteCmsById(int id)
          {
               await _repoCms.DeleteAsync(id);
          }

          /// <summary>
          /// Change Existence Of Cms Record name
          /// </summary>
          /// <param name="PageName">Page Name</param>
          /// <returns>True or False</returns>
          public bool IsExistCmsRecordName(string PageName)
          {
               int count = _repoCms.Query().Filter(x => x.PageName == PageName).Get().Count();
               if (count == 0)
                    return true;
               else
                    return false;
          }

          /// <summary>
          /// Dispose Cms Service
          /// </summary>
          public void Dispose()
          {

               if (_repoCms != null)
               {
                    _repoCms.Dispose();
               }
          }
     }
}
