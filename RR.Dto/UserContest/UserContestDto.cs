using System;

namespace RR.Dto
{
     public class UserContestDto
     {
          /// <summary>
          /// UserId
          /// </summary>
          public long? UserId { get; set; }

          public decimal? TeamPoint { get; set; }

          public int Rank { get; set; }  

          /// <summary>
          /// FullName
          /// </summary>
          public string Name { get; set; }
          /// <summary>
          /// Email ID 
          /// </summary>
          public string Email { get; set; }
          /// <summary>
          /// Phonenumber
          /// </summary>
          public string PhoneNumber { get; set; }
          /// <summary>
          /// Address
          /// </summary>
          public string Address { get; set; }
          /// <summary>
          /// Active Status
          /// </summary>
          public bool? IsActive { get; set; }

          /// <summary>
          /// ContestId
          /// </summary>
          public int ContestId { get; set; }
     }
}
