using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaRCEntryLiteDto
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }
        
        public int RowId { get; set; }

        public string CompetitorId { get; set; }

        public string CompetitorName { get; set; }

        public decimal RiderPower { get; set; }

        public decimal RiderPowerCurrent { get; set; }

        public decimal RiderPowerAvg { get; set; }

        public decimal CalcuttaPrice { get; set; }

        public bool IsSold { get; set; }

        public bool IsSolded { get; set; }

        public int CheckOutUsers { get; set; }

        public bool IsCheckOuted { get; set; }

        public int ShopCartId { get; set; }

        public decimal TotalWon { get; set; }
        public int RiderId { get; set; }
        public string RiderAvatar { get; set; }
    }

    public class CalcuttaRCResultLiteDto
    {
        public int Id { get; set; }
        public string ParentEventId { get; set; }

        public int RowId { get; set; }

        public string CompetitorId { get; set; }

        public string CompetitorName { get; set; }

        public decimal Score { get; set; }

        public int RiderId { get; set; }
        public string RiderAvatar { get; set; }

        public decimal CalcuttaPrice { get; set; }

        public string UserName { get; set; }

        public int IsSolded { get; set; }
        public decimal EarnMoney { get; set; }
        public int EarnRealPlace { get; set; }
        public string EarnPlace { get; set; }
    }
    public class CalcuttRCDetailDto
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

        public List<CalcuttaRCEntryLiteDto> EntryList { get; set; }
        public List<CalcuttaRCResultLiteDto> ResultList { get; set; }
        

    }
    public class CalcuttaCheckoutRCDetailDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
        public string ContestType { get; set; }
        public List<CalcuttaRCEntryLiteDto> EntryList { get; set; }
    }

}
