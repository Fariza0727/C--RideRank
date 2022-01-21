using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RR.StaticData
{
    public partial class RankRideStaticContext : DbContext
    {
        public RankRideStaticContext()
        {
        }

        public RankRideStaticContext(DbContextOptions<RankRideStaticContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bull> Bull { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventBull> EventBull { get; set; }
        public virtual DbSet<EventDraw> EventDraw { get; set; }
        public virtual DbSet<EventRider> EventRider { get; set; }
        public virtual DbSet<Rider> Rider { get; set; }
        public virtual DbSet<CalcuttaEvent> CalCuttaEvent { get; set; }
        public virtual DbSet<CalcuttaEventClass> CalCuttaEventClass { get; set; }
        public virtual DbSet<CalcuttaEventEntry> CalCuttaEventEntry { get; set; }
        public virtual DbSet<CalcuttaEventResult> CalCuttaEventResult { get; set; }
        public virtual DbSet<PayoutBasic> PayoutBasic { get; set; }

        public virtual DbSet<CalcuttaRC> CalcuttaRC { get; set; }
        public virtual DbSet<CalcuttaRCEntry> CalcuttaRCEntry { get; set; }
        public virtual DbSet<CalcuttaRCResult> CalcuttaRCResult { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=1TCDDCLKSX\\SQLEXPRESS;Database=RankRideStatic;User ID=usr_rankride;Password=Z7h=N!4Zkt;Persist Security Info=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Bull>(entity =>
            {
                entity.Property(e => e.AverageMark).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Brand)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BuckOffPerc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BuckOffPercVsLeftHandRider)
                    .HasColumnName("BuckOffPerc_VS_LeftHandRider")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BuckOffPercVsRightHandRider)
                    .HasColumnName("BuckOffPerc_VS_RightHandRider")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BuckOffPercVsTopRider)
                    .HasColumnName("BuckOffPerc_VS_TopRider")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OutVsLeftHandRider).HasColumnName("Out_VS_LeftHandRider");

                entity.Property(e => e.OutVsRightHandRider).HasColumnName("Out_VS_RightHandRider");

                entity.Property(e => e.OutsVsTopRiders).HasColumnName("Outs_VS_TopRiders");

                entity.Property(e => e.Owner)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PowerRating).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Pbrid)
                    .HasColumnName("PBRID")
                    .HasMaxLength(20);

                entity.Property(e => e.PerfTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Sanction)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WinningDistributed)
                    .IsRequired()
                    .HasDefaultValueSql("('false')");
            });

            modelBuilder.Entity<EventBull>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TierScore).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Bull)
                    .WithMany(p => p.EventBull)
                    .HasForeignKey(d => d.BullId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventBull_Bull");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventBull)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventBull_Event");
            });

            modelBuilder.Entity<EventDraw>(entity =>
            {
                entity.Property(e => e.BullName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RiderName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Round)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventDraw)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventDraw_Event");
            });

            modelBuilder.Entity<EventRider>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CwrpBonus)
                    .HasColumnName("Cwrp_Bonus")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.EventTierScore).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.EventRider)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventRider_Event");

                entity.HasOne(d => d.Rider)
                    .WithMany(p => p.EventRider)
                    .HasForeignKey(d => d.RiderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EventRider_Rider");
            });

            modelBuilder.Entity<Rider>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Cwrp).HasColumnName("CWRP");

                entity.Property(e => e.Hand)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RidePerc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RidePrecCurent).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RiderPower).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RiderPowerCurrent).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Streak).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<CalcuttaEvent>(entity =>
            {
                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ParentEventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ContestUTCLockTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.ContestType)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

            });

            modelBuilder.Entity<CalcuttaEventClass>(entity =>
            {
                entity.Property(e => e.EventClass)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ParentEventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EventType)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.EventLabel)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

            });
            modelBuilder.Entity<CalcuttaEventEntry>(entity =>
            {

                entity.Property(e => e.ParentEventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EntryId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

            });

            modelBuilder.Entity<CalcuttaEventResult>(entity =>
            {
                entity.Property(e => e.ParentEventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EventId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EntryId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OutId)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

            });

            modelBuilder.Entity<PayoutBasic>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

            });

            modelBuilder.Entity<CalcuttaRC>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<CalcuttaRCEntry>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
            modelBuilder.Entity<CalcuttaRCResult>(entity =>
            {
                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
        }
    }
}
