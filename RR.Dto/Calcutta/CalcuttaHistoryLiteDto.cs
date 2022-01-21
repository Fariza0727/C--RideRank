using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RR.Dto.Calcutta
{
    public class CalcuttaHistoryLiteDto
    {
        public int Id { get; set; }

        public DateTime ContestUTCLockTime { get; set; }

        public string EventName { get; set; }

        public string EventClassName { get; set; }

        public string Place { get; set; }

        public decimal Money { get; set; }

        public int RealPlace { get; set; }
        public bool IsFinished { get; set; }
        public string JoiningDateUTCString { get; set; }
    }

}
