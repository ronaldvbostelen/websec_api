using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets;
using Client_tech_resversi_api.Assets.Interfaces;
using Client_tech_resversi_api.Models.Interfaces;
using Client_tech_resversi_api.Models.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Client_tech_resversi_api.Models
{
    public partial class ReversiContext : DbContext
    {
        public ReversiContext()
        {
        }

        public ReversiContext(DbContextOptions<ReversiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Game> Game { get; set; }
        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<antiTempering> antiTempering { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAccount> UserAccounts { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserLastChanged> UserLastChanges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");


            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserAccount>().ToTable("UserAccount");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<UserLastChanged>().ToTable("UserLastChanged");

            modelBuilder.Entity<Game>(entity =>
            {
                entity.Property(e => e.gameId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.gameboard).IsUnicode(false);

                entity.HasOne(d => d.playerOneNavigation)
                    .WithMany(p => p.GameplayerOneNavigation)
                    .HasForeignKey(d => d.playerOne)
                    .HasConstraintName("FK_player");

                entity.HasOne(d => d.playerTwoNavigation)
                    .WithMany(p => p.GameplayerTwoNavigation)
                    .HasForeignKey(d => d.playerTwo)
                    .HasConstraintName("FK_player_two");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasIndex(e => e.username)
                    .HasName("unique_username")
                    .IsUnique();

                entity.Property(e => e.playerId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.email).IsUnicode(false);

                entity.Property(e => e.password).IsUnicode(false);

                entity.Property(e => e.salt).IsUnicode(false);

                entity.Property(e => e.screenname).IsUnicode(false);

                entity.Property(e => e.status).IsUnicode(false);

                entity.Property(e => e.username).IsUnicode(false);
            });

            modelBuilder.Entity<antiTempering>(entity =>
            {
                entity.Property(e => e.gameId).ValueGeneratedNever();

                entity.Property(e => e.state).IsUnicode(false);

                entity.HasOne(d => d.lastMoveNavigation)
                    .WithMany(p => p.antiTempering)
                    .HasForeignKey(d => d.lastMove)
                    .HasConstraintName("FK_antiTempering_Player");
            });

            modelBuilder.Entity<User>()
                .HasAlternateKey(a => a.Name);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(x => x.Password)
                .IsRequired();


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entities = ChangeTracker.Entries<IPossession>().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

            foreach (var entityEntry in entities)
            {
                var id = entityEntry.Entity.UserId;

                var lastChanged = await UserLastChanges.FirstOrDefaultAsync(x => x.UserId == id, cancellationToken: cancellationToken);

                if (lastChanged == null)
                {
                    lastChanged = new UserLastChanged {UserId = id};
                    Entry(lastChanged).State = EntityState.Added;
                }
                else
                {
                    if (Entry(lastChanged).State == EntityState.Unchanged)
                    {
                        Entry(lastChanged).State = EntityState.Modified;
                    }
                }

                lastChanged.DateTimeChanged = DateTime.Now.ToString();
            }


            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}