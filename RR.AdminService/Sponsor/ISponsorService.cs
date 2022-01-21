using RR.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RR.AdminService
{
     public interface ISponsorService : IDisposable
     {
          /// <summary>
          /// Get All Sponsors
          /// </summary>
          /// <param name="start">Page Number</param>
          /// <param name="length">Number Of Record </param>
          /// <param name="searchStr"></param>
          /// <param name="sort"></param>
          /// <returns></returns>
          Task<Tuple<IEnumerable<SponsorDto>, int>> GetAllSponsors(int start, int length, int column, string searchStr = "", string sort = "");

          /// <summary>
          /// Get Sponsor By Id
          /// </summary>
          /// <param name="sponsorId">Sponsor Id</param>
          /// <returns>The RiderDto</returns>
          Task<SponsorDto> GetSponsorById(int sponsorId);

          /// <summary>
          /// Add Update Sponsor Detail
          /// </summary>
          /// <param name="sponsorDto">The SponsorDto</param>
          /// <returns></returns>
          Task AddUpdateSponsorDetail(SponsorDto sponsorDto, string userId);

          /// <summary>
          /// Delete Sponsor By Id
          /// </summary>
          /// <param name="sponsorId">Sponsor Id</param>
          /// <returns></returns>
          Task DeleteSponsor(int sponsorId);
     }
}
