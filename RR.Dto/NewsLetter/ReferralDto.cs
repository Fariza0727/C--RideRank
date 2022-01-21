using System.ComponentModel.DataAnnotations;

namespace RR.Dto
{
     public class ReferralDto
     {
        [Required(ErrorMessage = "Friend name  is required.")]
        public string FriendName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required(ErrorMessage = "Please enter email.")]
        [EmailAddress(ErrorMessage = "Please enter valid email address.")]
        public string Email { get; set; }

     }
}
