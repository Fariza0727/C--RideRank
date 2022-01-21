using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace RR.Dto
{
     public class UserLiteDetailDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public long Id { get; set; }

          /// <summary>
          /// User Name
          /// </summary>
          public string UserName { get; set; }

          /// <summary>
          /// Email Address
          /// </summary>
          public string Email { get; set; }

          /// <summary>
          /// Phone Number
          /// </summary>
          public string PhoneNumber { get; set; }

          /// <summary>
          /// Is Active
          /// </summary>
          public bool IsActive { get; set; }

          /// <summary>
          /// Updated Date
          /// </summary>
          public string UpdatedDate { get; set; }

        public string LockoutEnd { get; set; }
        public bool IsEmailConfirm { get; set; }
        public string Membership { get; set; }
        public string Address { get; set; }
        public bool IsNotifyEmail { get; set; }
        public bool IsNotifySMS { get; set; }
    }
}
