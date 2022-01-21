using System;
using System.Collections.Generic;

namespace RR.Data
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
            AspNetUserTokens = new HashSet<AspNetUserTokens>();
            JoinedContest = new HashSet<JoinedContest>();
            LongTermTeam = new HashSet<LongTermTeam>();
            Team = new HashSet<Team>();
            SimpleTeam = new HashSet<SimpleTeam>();
            Transaction = new HashSet<Transaction>();
            UserChatMessagesConnectedUser = new HashSet<UserChatMessages>();
            UserChatMessagesUser = new HashSet<UserChatMessages>();
            UserDetail = new HashSet<UserDetail>();
            UserRequests = new HashSet<UserRequests>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<JoinedContest> JoinedContest { get; set; }
        public virtual ICollection<LongTermTeam> LongTermTeam { get; set; }
        public virtual ICollection<Team> Team { get; set; }
        public virtual ICollection<SimpleTeam> SimpleTeam { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
        public virtual ICollection<UserChatMessages> UserChatMessagesConnectedUser { get; set; }
        public virtual ICollection<UserChatMessages> UserChatMessagesUser { get; set; }
        public virtual ICollection<UserDetail> UserDetail { get; set; }
        public virtual ICollection<UserRequests> UserRequests { get; set; }
    }
}
