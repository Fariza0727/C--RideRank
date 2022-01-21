namespace RR.Dto.Team
{
     public class TeamRankDto
     {
          /// <summary>
          /// User id
          /// </summary>
          public string UserId { get; set; }

          /// <summary>
          /// Event Id
          /// </summary>
          public int EventId { get; set; }

          /// <summary>
          /// Contest Type
          /// </summary>
          public byte ContestType { get; set; }

          /// <summary>
          /// Team Rank
          /// </summary>
          public int Rank { get; set; }

          /// <summary>
          /// Team points
          /// </summary>
          public decimal TeamPoint { get; set; }

          /// <summary>
          /// joined contest Id
          /// </summary>
          public int ContestId { get; set; }

          /// <summary>
          /// joined Team Id
          /// </summary>
          public int TeamID { get; set; }
        /// <summary>
        /// Is Team PaidMember
        /// </summary>
        public bool IsTeamPaidMember { get; set; }
    }
}
