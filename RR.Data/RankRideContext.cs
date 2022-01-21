using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RR.Data
{
    public partial class RankRideContext : DbContext
    {
        public RankRideContext()
        {
        }

        public RankRideContext(DbContextOptions<RankRideContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<ContestUserWinner> ContestUserWinner { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<FavoriteBullRiders> FavoriteBullRiders { get; set; }
        public virtual DbSet<ForgetPasswordRequest> ForgetPasswordRequest { get; set; }
        public virtual DbSet<JoinedContest> JoinedContest { get; set; }
        public virtual DbSet<LongTermTeam> LongTermTeam { get; set; }
        public virtual DbSet<LongTermTeamBull> LongTermTeamBull { get; set; }
        public virtual DbSet<LongTermTeamRider> LongTermTeamRider { get; set; }
        public virtual DbSet<NewsLetterSubscribe> NewsLetterSubscribe { get; set; }
        public virtual DbSet<PasswordRequest> PasswordRequest { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<Team> Team { get; set; }
        public virtual DbSet<TeamBull> TeamBull { get; set; }
        public virtual DbSet<TeamRider> TeamRider { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<UserChatMessages> UserChatMessages { get; set; }
        public virtual DbSet<UserDetail> UserDetail { get; set; }
        public virtual DbSet<UserRequests> UserRequests { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCart { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistory { get; set; }

        public virtual DbSet<SimpleTeam> SimpleTeam { get; set; }
        public virtual DbSet<SimpleTeamBull> SimpleTeamBull { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=1TCDDCLKSX\\SQLEXPRESS;Database=RankRide;User ID=usr_rankride;Password=Z7h=N!4Zkt;Persist Security Info=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("IX_City")
                    .IsUnique();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ContestUserWinner>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.ContestUserWinner)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContestUserWinner_Team");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FavoriteBullRiders>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            modelBuilder.Entity<ForgetPasswordRequest>(entity =>
            {
                entity.Property(e => e.UrlId)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<JoinedContest>(entity =>
            {
                entity.HasKey(e => e.PaymentTxnId)
                    .HasName("PK_JoinedContest_1");

                entity.Property(e => e.PaymentTxnId).ValueGeneratedNever();

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.PaymentTxn)
                    .WithOne(p => p.JoinedContest)
                    .HasForeignKey<JoinedContest>(d => d.PaymentTxnId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JoinedContest_Transaction");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.JoinedContest)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JoinedContest_Team");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.JoinedContest)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JoinedContest_AspNetUsers");
            });

            modelBuilder.Entity<LongTermTeam>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ExpiredDate).HasColumnType("datetime");

                entity.Property(e => e.TeamBrand).HasMaxLength(500);

                entity.Property(e => e.TeamIcon).HasMaxLength(500);

                entity.Property(e => e.TeamPoint).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.LongTermTeam)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamLongTerm_AspNetUsers");
            });

            modelBuilder.Entity<LongTermTeamBull>(entity =>
            {
                entity.HasKey(e => e.TeamBullId);

                entity.Property(e => e.BonusPoint).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BullPoint).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.LongTermTeamBull)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LongTermTeamBull_LongTermTeam");
            });

            modelBuilder.Entity<LongTermTeamRider>(entity =>
            {
                entity.HasKey(e => e.TeamRiderId);

                entity.Property(e => e.BonusPoint).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RiderPoint).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.LongTermTeamRider)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LongTermTeamRider_LongTermTeam");
            });

            modelBuilder.Entity<NewsLetterSubscribe>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<PasswordRequest>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.UserEmail)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StateCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_State_Country");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TeamPoint)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Team)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Team_AspNetUsers");
            });

            modelBuilder.Entity<TeamBull>(entity =>
            {
                entity.Property(e => e.BullPoint)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("('0')");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamBull)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamBull_Team");
            });

            modelBuilder.Entity<TeamRider>(entity =>
            {
                entity.Property(e => e.RiderPoint)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("('0')");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamRider)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TeamRider_Team");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.AuthCode).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ResponseMessage).HasMaxLength(50);

                entity.Property(e => e.TextMessage).HasMaxLength(50);

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionDebit).HasColumnType("money");

                entity.Property(e => e.TransactionId)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Transaction_AspNetUsers");
            });

            modelBuilder.Entity<UserChatMessages>(entity =>
            {
                entity.Property(e => e.ConnectedUserId).HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastSeenDate).HasColumnType("datetime");

                entity.Property(e => e.Message)
                    .HasMaxLength(1000)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.ConnectedUser)
                    .WithMany(p => p.UserChatMessagesConnectedUser)
                    .HasForeignKey(d => d.ConnectedUserId)
                    .HasConstraintName("FK_UserChatMessages_AspNetUsers1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserChatMessagesUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserChatMessages_AspNetUsers");
            });

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.City).HasMaxLength(150);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.IsBlock).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentMode).HasMaxLength(50);

                entity.Property(e => e.PlayerType).HasMaxLength(50);

                entity.Property(e => e.ShopifyMembership).HasMaxLength(250);

                entity.Property(e => e.SubscriptionExpiryDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UserName).IsRequired();

                entity.Property(e => e.ZipCode).HasMaxLength(50);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserDetail)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UId");
            });

            modelBuilder.Entity<UserRequests>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Message).HasMaxLength(1500);

                entity.Property(e => e.RequestMessage).HasMaxLength(1500);

                entity.Property(e => e.RequestNo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.LongTermTeam)
                    .WithMany(p => p.UserRequests)
                    .HasForeignKey(d => d.LongTermTeamId)
                    .HasConstraintName("FK_UserRequests_LongTermTeam");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRequests_AspNetUsers");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(150);

            });

            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.Property(e => e.UserId).HasMaxLength(150);
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<SimpleTeam>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SimpleTeamPoint)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SimpleTeam)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SimpleTeam_AspNetUsers");
            });

            modelBuilder.Entity<SimpleTeamBull>(entity =>
            {
                entity.Property(e => e.CompetitorPoint)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("('0')");

                entity.HasOne(d => d.SimpleTeam)
                    .WithMany(p => p.SimpleTeamBull)
                    .HasForeignKey(d => d.SimpleTeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SimpleTeamBull_SimpleTeam");
            });
        }
    }
}
