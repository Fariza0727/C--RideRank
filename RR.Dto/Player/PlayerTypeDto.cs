using System;
using System.Collections.Generic;
using System.Text;

namespace RR.Dto.Player
{
    public class PlayerTypeDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }
    }


    
    public class PlayerLiteDto
    {
        public string Team { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Membership { get; set; }
        public string Address { get; set; }
        public bool IsNotifyEmail { get; set; }
        public bool IsNotifySMS { get; set; }
    }

}
