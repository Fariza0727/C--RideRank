using RR.AdminData;
using RR.Dto;
using RR.StaticData;
using System.Collections.Generic;
using System.Linq;

namespace RR.AdminMapper
{
     public static class SponsorMapper
     {
          /// <summary>
          /// Map List of Sponsor To SponsorDto
          /// </summary>
          /// <param name="sponsors"></param>
          /// <returns></returns>
          public static IEnumerable<SponsorDto> Map(IEnumerable<Sponsor> sponsors)
          {
               return sponsors.Select(p => MapDto(p));
          }

          /// <summary>
          /// MapDto
          /// </summary>
          /// <param name="sponsor">The Sponsor</param>
          /// <returns>The SponsorDto</returns>
          public static SponsorDto MapDto(Sponsor sponsor)
          {
               return new SponsorDto
               {
                    Id = sponsor.Id,
                    IsActive = sponsor.IsActive,
                    SponsorLogo = sponsor.SponsorLogo,
                    SponsorName = sponsor.SponsorName,
                    CreatedBy = sponsor.CreatedBy,
                    UpdatedDate = sponsor.UpdatedDate,
                    ShowImage= "/assets/SponsorLogo/" + sponsor.SponsorLogo,
                    WebUrl = sponsor.WebUrl
               };
          }
     }
}
