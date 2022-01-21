using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RR.Dto
{
    public class LongTermTeamDto
    {
        /// <summary>
        /// User id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Team Number
        /// </summary>
        public int TeamNumber { get; set; }

        /// <summary>
        /// Bull Id
        /// </summary>
        public int BullId { get; set; }

        /// <summary>
        /// RiderId
        /// </summary>
        public int RiderId { get; set; }

        /// <summary>
        /// Is Substitute
        /// </summary>
        public bool IsSubstitute { get; set; }

        /// <summary>
        /// TeamId
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Tier
        /// </summary>
        public int Tier { get; set; }


    }
}
