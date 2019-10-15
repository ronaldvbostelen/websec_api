using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Client_tech_resversi_api.Migrations
{
    public partial class schonelei : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Player",
                columns: table => new
                {
                    playerId = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    username = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    screenname = table.Column<string>(unicode: false, maxLength: 255, nullable: true),
                    password = table.Column<string>(unicode: false, maxLength: 2048, nullable: false),
                    salt = table.Column<string>(unicode: false, maxLength: 2048, nullable: false),
                    email = table.Column<string>(unicode: false, maxLength: 255, nullable: false),
                    verified = table.Column<bool>(nullable: false),
                    role = table.Column<int>(nullable: false),
                    status = table.Column<string>(unicode: false, maxLength: 1, nullable: false),
                    deleted = table.Column<bool>(nullable: false),
                    loginAttempt = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => x.playerId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    ScreenName = table.Column<string>(maxLength: 255, nullable: true),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Salt = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.UniqueConstraint("AK_User_Email", x => x.Email);
                    table.UniqueConstraint("AK_User_Name", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "antiTempering",
                columns: table => new
                {
                    gameId = table.Column<Guid>(nullable: false),
                    lastMove = table.Column<Guid>(nullable: true),
                    puckCount = table.Column<int>(nullable: true),
                    test = table.Column<string>(nullable: true),
                    state = table.Column<string>(unicode: false, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_antiTempering", x => x.gameId);
                    table.ForeignKey(
                        name: "FK_antiTempering_Player",
                        column: x => x.lastMove,
                        principalTable: "Player",
                        principalColumn: "playerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    gameId = table.Column<Guid>(nullable: false, defaultValueSql: "(newsequentialid())"),
                    playerOne = table.Column<Guid>(nullable: true),
                    playerTwo = table.Column<Guid>(nullable: true),
                    startTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    endTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    gameboard = table.Column<string>(unicode: false, maxLength: 2056, nullable: true),
                    winner = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.gameId);
                    table.ForeignKey(
                        name: "FK_player",
                        column: x => x.playerOne,
                        principalTable: "Player",
                        principalColumn: "playerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_player_two",
                        column: x => x.playerTwo,
                        principalTable: "Player",
                        principalColumn: "playerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Verified = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    LastTimeLoggedIn = table.Column<string>(nullable: true),
                    LastTimeLoggedInFrom = table.Column<string>(nullable: true),
                    LoginAttempts = table.Column<int>(nullable: false),
                    ActivationKey = table.Column<string>(nullable: true),
                    RecoverKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccount_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Claim = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Issuer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLastChanged",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    DateTimeChanged = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLastChanged", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLastChanged_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_antiTempering_lastMove",
                table: "antiTempering",
                column: "lastMove");

            migrationBuilder.CreateIndex(
                name: "IX_Game_playerOne",
                table: "Game",
                column: "playerOne");

            migrationBuilder.CreateIndex(
                name: "IX_Game_playerTwo",
                table: "Game",
                column: "playerTwo");

            migrationBuilder.CreateIndex(
                name: "unique_username",
                table: "Player",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_UserId",
                table: "UserAccount",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLastChanged_UserId",
                table: "UserLastChanged",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "antiTempering");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "UserAccount");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserLastChanged");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Player");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
