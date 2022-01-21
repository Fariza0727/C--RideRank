using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RR.AdminData
{
    public partial class RankRideAdminContext : DbContext
    {
        public RankRideAdminContext()
        {
        }

        public RankRideAdminContext(DbContextOptions<RankRideAdminContext> options)
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
        public virtual DbSet<Award> Award { get; set; }
        public virtual DbSet<AwardType> AwardType { get; set; }
        public virtual DbSet<Banner> Banner { get; set; }
        public virtual DbSet<Cms> Cms { get; set; }
        public virtual DbSet<Contest> Contest { get; set; }
        public virtual DbSet<ContestCategory> ContestCategory { get; set; }
        public virtual DbSet<ContestWinner> ContestWinner { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<PageDetail> PageDetail { get; set; }
        public virtual DbSet<PagePermission> PagePermission { get; set; }
        public virtual DbSet<PicuresManager> PicuresManager { get; set; }
        public virtual DbSet<PointTable> PointTable { get; set; }
        public virtual DbSet<RiderManager> RiderManager { get; set; }
        public virtual DbSet<Sponsor> Sponsor { get; set; }
        public virtual DbSet<UsersLog> UsersLog { get; set; }
        public virtual DbSet<VideoSlider> VideoSlider { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=1TCDDCLKSX\\SQLEXPRESS;Database=RankRideAdmin;User ID=usr_rankride;Password=Z7h=N!4Zkt;Persist Security Info=true;");
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

            modelBuilder.Entity<Award>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(500);

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.Token).HasMaxLength(300);

                entity.HasOne(d => d.AwardType)
                    .WithMany(p => p.Award)
                    .HasForeignKey(d => d.AwardTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Award_AwardType");
            });

            modelBuilder.Entity<AwardType>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(300)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PicPath).HasMaxLength(400);

                entity.Property(e => e.Title).HasMaxLength(400);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(200);
            });

            modelBuilder.Entity<Cms>(entity =>
            {
                entity.ToTable("CMS");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MetaDescription)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.MetaKeyword)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.MetaTitle)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PageContent).IsRequired();

                entity.Property(e => e.PageName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PageUrl)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Contest>(entity =>
            {
                entity.Property(e => e.CreatdDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.JoiningFee).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UniqueCode)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.WinningPrice).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.WinningTitle)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.HasOne(d => d.ContestCategory)
                    .WithMany(p => p.Contest)
                    .HasForeignKey(d => d.ContestCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contest_Category");
            });

            modelBuilder.Entity<ContestCategory>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ContestWinner>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Contest)
                    .WithMany(p => p.ContestWinner)
                    .HasForeignKey(d => d.ContestId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContestWinner_Contest");

                entity.HasOne(d => d.MarchendiseNavigation)
                    .WithMany(p => p.ContestWinner)
                    .HasForeignKey(d => d.Marchendise)
                    .HasConstraintName("FK_ContestWinner_Award");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasMaxLength(450);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.NewsContent).IsRequired();

                entity.Property(e => e.NewsDate).HasColumnType("datetime");

                entity.Property(e => e.NewsTag)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PicPath).HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.VideoPath).HasMaxLength(500);

                entity.Property(e => e.VideoUrl).HasMaxLength(1000);
            });

            modelBuilder.Entity<PageDetail>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PageBaseUrl)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.PageName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PageUrl).IsRequired();
            });

            modelBuilder.Entity<PagePermission>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.PagePermission)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PagePermission_PageId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PagePermission)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PagePermission_UserId");
            });

            modelBuilder.Entity<PicuresManager>(entity =>
            {
                entity.Property(e => e.BullName).HasMaxLength(500);

                entity.Property(e => e.BullPicture).HasMaxLength(500);

                entity.Property(e => e.RiderName).HasMaxLength(500);

                entity.Property(e => e.RiderPicture).HasMaxLength(500);
            });

            modelBuilder.Entity<PointTable>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(1000);
            });

            modelBuilder.Entity<RiderManager>(entity =>
            {
                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Sociallink).HasMaxLength(500);

                entity.Property(e => e.Type).HasMaxLength(100);
            });

            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.Property(e => e.CreatedBy).HasMaxLength(450);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SponsorLogo)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SponsorName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebUrl).HasMaxLength(500);
            });

            modelBuilder.Entity<UsersLog>(entity =>
            {
                entity.Property(e => e.LogDate).HasColumnType("datetime");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<VideoSlider>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(250);

                entity.Property(e => e.VideoPath)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.VideoThumb).HasMaxLength(500);

                entity.Property(e => e.VideoUrl).HasMaxLength(500);
            });
        }
    }
}
