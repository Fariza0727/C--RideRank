using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto.Contest
{
    public class ContestWinnerLiteDto
    {
        /// <summary>
        /// Price Percentage
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Token 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Marchendise
        /// </summary>
        public string Merchandise { get; set; }

        /// <summary>
        /// Token Percentage
        /// </summary>
        public string OtherReward { get; set; }
    }
}
