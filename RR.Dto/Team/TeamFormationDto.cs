using System.Collections.Generic;

namespace RR.Dto.Team
{
     public class TeamFormationDto
     {
          public string UserId { get; set; }
          public int EventId { get; set; }
          public int ContestId { get; set; }
          public int TeamNumber { get; set; }
          public List<TeamBullFormationDto> BullList { get; set; }
          public List<TeamRiderFormationDto> RiderList { get; set; }
          public bool IsFinished { get; set; }
          public bool IsAlreadyJoined { get; set; }
          public bool IsEditedTeam { get; set; }
          public DataResult<List<TeamBullFormationDto>, List<TeamBullFormationDto>, List<TeamBullFormationDto>> BullArray { get; set; }
          public DataResult<List<TeamRiderFormationDto>, List<TeamRiderFormationDto>, List<TeamRiderFormationDto>> RiderArray { get; set; }
          public int BullTier1 { get; set; }
          public int BullTier2 { get; set; }
          public int BullTier3 { get; set; }
          public int[] RiderTier1 { get; set; }
          public int[] RiderTier2 { get; set; }
          public int[] RiderTier3 { get; set; }
     }
     public class TeamBullFormationDto
     {
          public int? TeamId { get; set; }
          public int BullId { get; set; }
          public string BullName { get; set; }
          public string BullAvatar { get; set; }
          public int BullTier { get; set; }
          public bool IsSelected { get; set; }
        public bool IsDropout { get; set; }
    }
     public class TeamRiderFormationDto
     {
          public int? TeamId { get; set; }
          public int RiderId { get; set; }
          public string RiderName { get; set; }
          public string RiderAvatar { get; set; }
          public int RiderTier { get; set; }
          public bool IsSelected { get; set; }
        public bool IsDropout { get; set; }
     }
}
