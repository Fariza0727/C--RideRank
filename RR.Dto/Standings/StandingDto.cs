using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto
{
     public class StandingDto
     {
          /// <summary>
          /// UserId
          /// </summary>
          public string UserId { get; set; }

          /// <summary>
          /// Team Rank
          /// </summary>
          public int Rank { get; set; }

          /// <summary>
          /// UserName
          /// </summary>
          public string UserName { get; set; }

          /// <summary>
          /// Team points
          /// </summary>
          public decimal TeamPoint { get; set; }

          /// <summary>
          /// Number Of Contest
          /// </summary>
          public int NumberOfContest { get; set; }

          /// <summary>
          /// PlayerType
          /// </summary>
          public string PlayerType { get; set; }

          /// <summary>
          /// TeamId
          /// </summary>
          public int TeamId { get; set; }

          /// <summary>
          /// Avtar
          /// </summary>
          public string Avtar { get; set; }
     }
}
