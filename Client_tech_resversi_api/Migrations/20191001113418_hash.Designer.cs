﻿// <auto-generated />
using System;
using Client_tech_resversi_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Client_tech_resversi_api.Migrations
{
    [DbContext(typeof(ReversiContext))]
    [Migration("20191001113418_hash")]
    partial class hash
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Client_tech_resversi_api.Models.Game", b =>
                {
                    b.Property<Guid>("gameId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newsequentialid())");

                    b.Property<DateTime?>("endTime")
                        .HasColumnType("datetime");

                    b.Property<string>("gameboard")
                        .HasMaxLength(2056)
                        .IsUnicode(false);

                    b.Property<Guid?>("playerOne");

                    b.Property<Guid?>("playerTwo");

                    b.Property<DateTime?>("startTime")
                        .HasColumnType("datetime");

                    b.Property<Guid?>("winner");

                    b.HasKey("gameId");

                    b.HasIndex("playerOne");

                    b.HasIndex("playerTwo");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Player", b =>
                {
                    b.Property<Guid>("playerId")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(newsequentialid())");

                    b.Property<bool>("deleted");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<int>("loginAttempt");

                    b.Property<string>("password")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .IsUnicode(false);

                    b.Property<int>("role");

                    b.Property<string>("salt")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .IsUnicode(false);

                    b.Property<string>("screenname")
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<string>("status")
                        .IsRequired()
                        .HasMaxLength(1)
                        .IsUnicode(false);

                    b.Property<string>("username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false);

                    b.Property<bool>("verified");

                    b.HasKey("playerId");

                    b.HasIndex("username")
                        .IsUnique()
                        .HasName("unique_username");

                    b.ToTable("Player");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("Salt");

                    b.Property<string>("ScreenName")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasAlternateKey("Email");

                    b.HasAlternateKey("Name");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActivationKey");

                    b.Property<string>("LastTimeLoggedIn");

                    b.Property<string>("LastTimeLoggedInFrom");

                    b.Property<int>("LoginAttempts");

                    b.Property<string>("RecoverKey");

                    b.Property<string>("SecurityHash");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasConversion(new ValueConverter<string, string>(v => default(string), v => default(string), new ConverterMappingHints(size: 1)));

                    b.Property<int>("UserId");

                    b.Property<bool>("Verified");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserAccount");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserClaim", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Claim");

                    b.Property<string>("Issuer");

                    b.Property<int>("UserId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaim");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserLastChanged", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DateTimeChanged");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserLastChanged");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Role");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.antiTempering", b =>
                {
                    b.Property<Guid>("gameId");

                    b.Property<Guid?>("lastMove");

                    b.Property<int?>("puckCount");

                    b.Property<string>("state")
                        .IsRequired()
                        .HasConversion(new ValueConverter<string, string>(v => default(string), v => default(string), new ConverterMappingHints(size: 1)))
                        .IsUnicode(false);

                    b.Property<string>("test");

                    b.HasKey("gameId");

                    b.HasIndex("lastMove");

                    b.ToTable("antiTempering");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Game", b =>
                {
                    b.HasOne("Client_tech_resversi_api.Models.Player", "playerOneNavigation")
                        .WithMany("GameplayerOneNavigation")
                        .HasForeignKey("playerOne")
                        .HasConstraintName("FK_player");

                    b.HasOne("Client_tech_resversi_api.Models.Player", "playerTwoNavigation")
                        .WithMany("GameplayerTwoNavigation")
                        .HasForeignKey("playerTwo")
                        .HasConstraintName("FK_player_two");
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserAccount", b =>
                {
                    b.HasOne("Client_tech_resversi_api.Models.Principal.User", "User")
                        .WithOne("UserAccount")
                        .HasForeignKey("Client_tech_resversi_api.Models.Principal.UserAccount", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserClaim", b =>
                {
                    b.HasOne("Client_tech_resversi_api.Models.Principal.User", "User")
                        .WithMany("UserClaims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserLastChanged", b =>
                {
                    b.HasOne("Client_tech_resversi_api.Models.Principal.User", "User")
                        .WithOne("UserLastChanged")
                        .HasForeignKey("Client_tech_resversi_api.Models.Principal.UserLastChanged", "UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.Principal.UserRole", b =>
                {
                    b.HasOne("Client_tech_resversi_api.Models.Principal.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Client_tech_resversi_api.Models.antiTempering", b =>
                {
                    b.HasOne("Client_tech_resversi_api.Models.Player", "lastMoveNavigation")
                        .WithMany("antiTempering")
                        .HasForeignKey("lastMove")
                        .HasConstraintName("FK_antiTempering_Player");
                });
#pragma warning restore 612, 618
        }
    }
}
