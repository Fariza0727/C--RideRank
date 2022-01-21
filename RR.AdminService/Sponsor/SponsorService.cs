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
     public class SponsorService : ISponsorService
     {
          #region Constructor 

          private readonly IRepository<Sponsor, RankRideAdminContext> _repoSponsor;

          public SponsorService(IRepository<Sponsor, RankRideAdminContext> repoSponsor)
          {
               _repoSponsor = repoSponsor;
          }

          #endregion

          /// <summary>
          /// Get All Sponsors
          /// </summary>
          /// <param name="start">Page Number</param>
          /// <param name="length">Number Of Record </param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <returns></returns>
          public async Task<Tuple<IEnumerable<SponsorDto>, int>> GetAllSponsors(int start, int length, int column, string searchStr = "", string sort = "")
          {
               int count = 0;

               var predicate = PredicateBuilder.True<Sponsor>()
           .And(x => (searchStr == "" || x.SponsorName.Contains(searchStr.ToLower())));

               var sponsors = _repoSponsor
                   .Query()
                   .Filter(predicate);

               if (FilterSortingVariable.SPONSOR_ID == column)
               {
                    sponsors = (sort == "desc" ? sponsors.OrderBy(x => x.OrderByDescending(xx => xx.Id)) : sponsors.OrderBy(x => x.OrderBy(xx => xx.Id)));
               }
               if (FilterSortingVariable.SPONSOR_NAME == column)
               {
                    sponsors = (sort == "desc" ? sponsors.OrderBy(x => x.OrderByDescending(xx => xx.SponsorName)) : sponsors.OrderBy(x => x.OrderBy(xx => xx.SponsorName)));
               }

               return await Task.FromResult(new Tuple<IEnumerable<SponsorDto>, int>(sponsors
                        .GetPage(start, length, out count).Select(y => new SponsorDto
                        {
                             Id = y.Id,
                             IsActive = y.IsActive,
                             SponsorLogo = y.SponsorLogo,
                             SponsorName = y.SponsorName,
                             WebUrl = y.WebUrl
                        }), count));
          }

          /// <summary>
          /// Get Sponsor By Id
          /// </summary>
          /// <param name="sponsorId">Sponsor Id</param>
          /// <returns>The RiderDto</returns>
          public async Task<SponsorDto> GetSponsorById(int sponsorId)
          {
               var sponsorDetail = _repoSponsor.Query()
                .Filter(e => e.Id == sponsorId)
                .Get()
                .SingleOrDefault();
               return await Task.FromResult(SponsorMapper.MapDto(sponsorDetail));
          }

          /// <summary>
          /// Update Sponsor Detail
          /// </summary>
          /// <param name="sponsorDto">The SponsorDto</param>
          /// <returns></returns>
          public async Task AddUpdateSponsorDetail(SponsorDto sponsorDto, string userId)
          {
               var sponsorData = _repoSponsor.Query()
                    .Filter(x => x.Id == sponsorDto.Id)
                    .Get()
                    .SingleOrDefault();

            

            if (sponsorData != null)
               {
                if (!string.IsNullOrEmpty(sponsorDto.SponsorLogo))
                    sponsorData.SponsorLogo = sponsorDto.SponsorLogo;

                sponsorData.IsActive = sponsorDto.IsActive;
                    sponsorData.SponsorName = !string.IsNullOrEmpty(sponsorDto.SponsorName) ? sponsorDto.SponsorName : "";
                    sponsorData.UpdatedBy = userId;
                    sponsorData.UpdatedDate = DateTime.Now;
                    sponsorData.WebUrl = sponsorDto.WebUrl;
                    sponsorData.IsActive = sponsorDto.IsActive;
                    await _repoSponsor.UpdateAsync(sponsorData);
               }
               else
               {
                    sponsorData = new Sponsor
                    {
                         IsActive = true,
                         SponsorName = !string.IsNullOrEmpty(sponsorDto.SponsorName) ? sponsorDto.SponsorName : "",
                         CreatedBy = userId,
                         UpdatedBy = userId,
                         UpdatedDate = DateTime.Now,
                         CreatedDate = DateTime.Now,
                        WebUrl = sponsorDto.WebUrl
            };
                if (!string.IsNullOrEmpty(sponsorDto.SponsorLogo))
                    sponsorData.SponsorLogo = sponsorDto.SponsorLogo;


                await _repoSponsor.InsertGraphAsync(sponsorData);
               }
          }

          /// <summary>
          /// Delete Sponsor By Id
          /// </summary>
          /// <param name="sponsorId">Sponsor Id</param>
          /// <returns></returns>
          public async Task DeleteSponsor(int sponsorId)
          {
               await _repoSponsor.DeleteAsync(sponsorId);
          }

          /// <summary>
          /// Dispose Sponsor Service
          /// </summary>
          public void Dispose()
          {
               if (_repoSponsor != null)
               {
                    _repoSponsor.Dispose();
               }
          }
     }
}
