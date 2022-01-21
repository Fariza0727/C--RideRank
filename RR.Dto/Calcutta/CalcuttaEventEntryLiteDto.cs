using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaEventEntryLiteDto
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }
        
        public string EventId { get; set; }

        public string EntryId { get; set; }

        public string RegNo { get; set; }

        public string CompetitorId { get; set; }

        public string CompetitorName { get; set; }

        public string Owner { get; set; }

        public string Handler { get; set; }

        public string Del { get; set; }

        public string Draw { get; set; }

        public decimal CalcuttaPrice { get; set; }

        public List<CalcuttaEventEntryStandingDto> Standings { get; set; }

        public string EventClassName { get; set; }

        public bool IsSold { get; set; }

        public bool IsSolded { get; set; }

        public int CheckOutUsers { get; set; }

        public bool IsCheckOuted { get; set; }

        public int ShopCartId { get; set; }

        public decimal TotalWon { get; set; }
    }

    public class CalcuttaEventResultLiteDto
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }

        public string EventId { get; set; }

        public string EntryId { get; set; }

        public string OutId { get; set; }

        public string RegNo { get; set; }

        public string CompetitorId { get; set; }

        public string CompetitorName { get; set; }

        public string Owner { get; set; }

        public string Del { get; set; }

        public decimal Score { get; set; }

        public string Place { get; set; }

        public decimal Money { get; set; }

        public string EventLinkId { get; set; }

        public int RealPlace { get; set; }
        public decimal CalcuttaPrice { get; set; }
        public string UserName { get; set; }

        public int IsSolded { get; set; }
        public decimal EarnMoney { get; set; }
        public int EarnRealPlace { get; set; }
        public string EarnPlace { get; set; }
    }
    public class CalcuttaDetailDto
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

        public List<CalcuttaEventEntryLiteDto> EntryList { get; set; }
        public List<CalcuttaEventResultLiteDto> ResultList { get; set; }
        public List<CalcuttaEventClassDto> ClassList { get; set; }

    }
    public class CalcuttaCheckoutDetailDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public string ContestType { get; set; }
        public List<CalcuttaEventEntryLiteDto> EntryList { get; set; }
    }

    public class PayoutBasicCalDto
    {
        public int Position { get; set; }
        public decimal PayoutPrice { get; set; }
    }
    public class EarngingDto
    {
        public string EventId { get; set; }
        public string EntryId { get; set; }
        public string EarnPlace { get; set; }
        public int EarnRealPlace { get; set; }
        public decimal EarningPrice { get; set; }

        public string Place { get; set; }
        public int RealPlace { get; set; }
        public int RowId { get; set; }
    }
}
