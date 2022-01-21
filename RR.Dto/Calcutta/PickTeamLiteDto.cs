using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class SimpleTeamDto
    {
        public string UserId { get; set; }
        public string TeamId { get; set; }
        public string CompetitorId { get; set; }
        public int EventId { get; set; }

    }
    public class PickEntryLiteDto
    {
        public string CompetitorId { get; set; }
        public string CompetitorName { get; set; }
        public string Owner { get; set; }

        public bool IsSelected { get; set; }
        public decimal TotalWon { get; set; }

        public decimal CompetitorPoint { get; set; }
    }

    public class PickTeamDetailDto
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ContestType { get; set; }
        public string ContestStatus { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ContestUTCLockTime { get; set; }
        public bool IsFinished { get; set; }

        public int CheckOutedCount { get; set; }

        public List<PickEntryLiteDto> EntryList { get; set; }

        public int TeamId { get; set; }
        
    }
    
}
