using RR.Dto.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Team
{
    public class TeamFormationDetailDto
    {
        public string UserId { get; set; }
        public int EventId { get; set; }
        public int ContestId { get; set; }
        public int TeamNumber { get; set; }
        public bool IsFinished { get; set; }
        public bool IsAlreadyJoined { get; set; }
        public bool IsEditedTeam { get; set; }
        public List<TeamBullDrawDto> BullList { get; set; }
        public List<TeamDrawDto> DrawList { get; set; }
        public List<EventResultDto> EventResultList { get; set; }
    }

    public class TeamBullDrawDto
    {
        public int? TeamId { get; set; }
        public int BullId { get; set; }
        public string BullName { get; set; }
        public string BullAvatar { get; set; }
        public int BullTier { get; set; }
        public bool BullIsSelected { get; set; }
        public bool BullIsDropout { get; set; }

        public string BullOwner { get; set; }
        public decimal BullAverageMark { get; set; }
        public decimal BullPowerRating { get; set; }
        public decimal BullRankRideScore { get; set; }
    }
    public class TeamRiderDrawDto
    {
        public int? TeamId { get; set; }
        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string RiderAvatar { get; set; }
        public int RiderTier { get; set; }
        public bool RiderIsSelected { get; set; }
        public bool RiderIsDropout { get; set; }
        public int WorldRanking { get; set; }
        public decimal RiderPower { get; set; }
        public decimal RRTotalpoint { get; set; }
    }
    public class TeamDrawDto
    {
        public string Round { get; set; }
        public decimal QRProb { get; set; }
        public int EventId { get; set; }

        public int RiderId { get; set; }
        public string RiderName { get; set; }
        public string RiderAvatar { get; set; }
        public int RiderTier { get; set; }
        public bool RiderIsSelected { get; set; }
        public bool RiderIsDropout { get; set; }
        public int WorldRanking { get; set; }
        public decimal RiderPower { get; set; }
        public decimal RRTotalpoint { get; set; }
        
        public int BullId { get; set; }
        public string BullName { get; set; }
        public string BullAvatar { get; set; }
        public int BullTier { get; set; }
        public bool BullIsSelected { get; set; }
        public bool BullIsDropout { get; set; }
        public string BullOwner { get; set; }
        public decimal BullAverageMark { get; set; }
        public decimal BullPowerRating { get; set; }
        public decimal BullRankRideScore { get; set; }
    }
}
