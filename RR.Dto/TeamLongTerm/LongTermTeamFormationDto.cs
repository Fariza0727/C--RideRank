using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RR.Dto.Team
{
    public class LongTermTeamFormationDto
    {
        public LongTermTeamFormationDto()
        {
            this.BullList = new List<LTTeamBullFormationDto>();
            this.RiderList = new List<LTTeamRiderFormationDto>();
        }
        public string UserId { get; set; }
        public IEnumerable<LTTeamBullFormationDto> BullList { get; set; }
        public IEnumerable<LTTeamRiderFormationDto> RiderList { get; set; }
        public bool IsEditedTeam { get; set; }
        public DataResult<List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>> BullArray { get; set; }
        public DataResult<List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>> RiderArray { get; set; }
        public int BullTier1 { get; set; }
        public int BullTier2 { get; set; }
        public int BullTier3 { get; set; }
        public int[] RiderTier1 { get; set; }
        public int[] RiderTier2 { get; set; }
        public int[] RiderTier3 { get; set; }
        public int TeamNumber { get; set; }
        public int totalRiders {get;set;}
        public int totalBulls { get;set;}
        public int TeamId { get; set; }
        public string TeamAvatar { get; set; }
        public string TeamBrand { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public IFormFile Icon { get; set; }
    }

    public class LongTermTeamFormationApiDto
    {
        public LongTermTeamFormationApiDto()
        {
            this.BullList = new List<LTTeamBullFormationDto>();
            this.RiderList = new List<LTTeamRiderFormationDto>();
        }
        public string UserId { get; set; }
        public IEnumerable<LTTeamBullFormationDto> BullList { get; set; }
        public IEnumerable<LTTeamRiderFormationDto> RiderList { get; set; }
        public bool IsEditedTeam { get; set; }
        public DataResult<List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>> BullArray { get; set; }
        public DataResult<List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>> RiderArray { get; set; }
        public string BullTier1 { get; set; }
        public string BullTier2 { get; set; }
        public string BullTier3 { get; set; }
        public string RiderTier1 { get; set; }
        public string RiderTier2 { get; set; }
        public string RiderTier3 { get; set; }
        public int TeamNumber { get; set; }
        public int totalRiders { get; set; }
        public int totalBulls { get; set; }
        public int TeamId { get; set; }
        public string TeamAvatar { get; set; }
        public string TeamBrand { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public IFormFile Icon { get; set; }
    }

    public class LongTermTeamFormationLiteDto
    {
        public LongTermTeamFormationLiteDto()
        {
            this.BullList = new List<LTTeamBullFormationDto>();
            this.RiderList = new List<LTTeamRiderFormationDto>();
        }
        public string UserId { get; set; }
        public IEnumerable<LTTeamBullFormationDto> BullList { get; set; }
        public IEnumerable<LTTeamRiderFormationDto> RiderList { get; set; }
        public int TeamNumber { get; set; }
        public int totalRiders { get; set; }
        public int totalBulls { get; set; }
        public int TeamId { get; set; }
        public string TeamAvatar { get; set; }
        public string TeamBrand { get; set; }

        public DataResult<List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>, List<LTTeamBullFormationDto>> BullArray { get; set; }
        public DataResult<List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>, List<LTTeamRiderFormationDto>> RiderArray { get; set; }

        public int BullTier1 { get; set; }
        public int BullTier2 { get; set; }
        public int BullTier3 { get; set; }
        public int[] RiderTier1 { get; set; }
        public int[] RiderTier2 { get; set; }
        public int[] RiderTier3 { get; set; }
    }

    public class LTTeamBullFormationDto
    {
        public int? TeamId { get; set; } 
        public int BullId { get; set; }
        public string BullName { get; set; }
        public string BullAvatar { get; set; }
        public int? BullTier { get; set; }
        public bool IsSelected { get; set; }
    }
    public class LTTeamRiderFormationDto
    {
        public int? TeamId { get; set; }
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string RiderAvatar { get; set; }
        public int? RiderTier { get; set; }
        public bool IsSelected { get; set; }
    }

}
