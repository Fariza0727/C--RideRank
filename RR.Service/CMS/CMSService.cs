using RR.AdminData;
using RR.Dto;
using RR.Mapper;
using RR.Repo;
using System.Linq;
using System.Threading.Tasks;

namespace RR.Service.CMS
{
     public class CMSService : ICMSService
     {
          #region Constructor

          private readonly IRepository<Cms, RankRideAdminContext> _repoCms;

          public CMSService(IRepository<Cms, RankRideAdminContext> repoCms)
          {
               _repoCms = repoCms;
          }

          #endregion

          /// <summary>
          /// Get Page Content By PageName
          /// </summary>
          /// <param name="page">The page</param>
          /// <returns>The Cms</returns>
          public async Task<CmsDto> GetPageContentByPageName(string page)
          {
               var cms = _repoCms.Query()
                  .Filter(o => o.PageName.ToLower() == page.ToLower() || o.PageUrl.ToLower() == page.ToLower())
                  .Get()
                  .SingleOrDefault();
               return await Task.FromResult(CMSMapper.MapDto(cms));
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
