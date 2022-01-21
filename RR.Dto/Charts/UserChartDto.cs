using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public class UserChartDto
    {
        public string title { get; set; }
        public string[] labels { get; set; }
        public int[] series { get; set; }
        public string parcentage { get; set; }
        public int high { get; set; }
    }
}
