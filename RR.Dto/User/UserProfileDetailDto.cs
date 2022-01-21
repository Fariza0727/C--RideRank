using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class UserProfileDetailDto
     {
          /// <summary>
          /// Id
          /// </summary>
          public long Id { get; set; }

          /// <summary>
          /// User Id
          /// </summary>
          public string UserId { get; set; }

          /// <summary>
          /// User Name
          /// </summary>
          public string UserName { get; set; }

          /// <summary>
          /// First Name
          /// </summary>
          public string FirstName { get; set; }

          /// <summary>
          /// Last Name
          /// </summary>
          public string LastName { get; set; }

          /// <summary>
          /// Email Address
          /// </summary>
          public string Email { get; set; }

          /// <summary>
          /// Phone Number
          /// </summary>
          public string PhoneNumber { get; set; }

          /// <summary>
          /// Address 
          /// </summary>
          public string Address1 { get; set; }

          /// <summary>
          /// Another Address
          /// </summary>
          public string Address2 { get; set; }

          /// <summary>
          /// Another Address
          /// </summary>
          public string Address3 { get; set; }

          /// <summary>
          /// City
          /// </summary>
          public string City { get; set; }

          /// <summary>
          /// State
          /// </summary>
          public int? State { get; set; }

          /// <summary>
          /// Country
          /// </summary>
          public int? Country { get; set; }

          /// <summary>
          /// Country Name
          /// </summary>
          public string CountryName { get; set; }

          /// <summary>
          /// State Name
          /// </summary>
          public string StateName { get; set; }

          /// <summary>
          /// City Name
          /// </summary>
          public string CityName { get; set; }

          /// <summary>
          /// Banking
          /// </summary>
          public string Banking { get; set; }

          /// <summary>
          /// Avtar
          /// </summary>
          public string Avtar { get; set; }

          /// <summary>
          /// League Notification
          /// </summary>
          public bool LeagueNotification { get; set; }

          /// <summary>
          /// Is Block
          /// </summary>
          public bool IsBlock { get; set; }

          /// <summary>
          /// Is Active
          /// </summary>
          public bool IsActive { get; set; }

          /// <summary>
          /// Is Delete
          /// </summary>
          public bool IsDelete { get; set; }

          /// <summary>
          /// Created Date
          /// </summary>
          public DateTime CreatedDate { get; set; }

          /// <summary>
          /// Updated Date
          /// </summary>
          public DateTime? UpdatedDate { get; set; }

          /// <summary>
          /// Date of birth
          /// </summary>
          public string DateOfBirth { get; set; }

          /// <summary>
          /// Country list
          /// </summary>
          public List<SelectListItem> CountryList { get; set; }

          /// <summary>
          /// State List
          /// </summary>
          public List<SelectListItem> StateList { get; set; }

          /// <summary>
          /// City List
          /// </summary>
          public List<SelectListItem> CityList { get; set; }

          /// <summary>
          /// Player Type
          /// </summary>
          public string PlayerType { get; set; }

          /// <summary>
          /// PlayerTypeId
          /// </summary>
          public string PlayerTypeId { get; set; }

          /// <summary>
          /// PlayerTypeList
          /// </summary>
          public List<SelectListItem> PlayerTypeList { get; set; }

          public int? WalletToken { get; set; }

          public DateTime? SubscriptionExpiryDate { get; set; }
     }
}
