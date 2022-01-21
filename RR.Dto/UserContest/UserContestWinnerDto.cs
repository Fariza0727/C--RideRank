using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
   public  class UserContestWinnerDto
    {
        /// <summary>
        /// Contest Id
        /// </summary>
        public long  ContestId { get; set; }
        /// <summary>
        /// Player name 
        /// </summary>
        public string PlayerName { get; set; }
        /// <summary>
        /// Player Email ID
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Player Team Point
        /// </summary>
        public decimal TeamPoint { get; set; }
        /// <summary>
        /// Player Team Rank
        /// </summary>
        public int TeamRank { get; set; }       
        /// <summary>
        /// Merchaendise types
        /// </summary>
        public string Marchendise { get; set; }
        /// <summary>
        /// Other Award Types
        /// </summary>
        public string Others { get; set; }
        /// <summary>
        /// Winner Tokens
        /// </summary>
        public string WinningToken { get; set; }
        /// <summary>
        /// Winner Price
        /// </summary>
        public string WinningPrice { get; set; }

    }
}
