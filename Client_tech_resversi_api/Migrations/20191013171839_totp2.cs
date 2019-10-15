using Microsoft.EntityFrameworkCore.Migrations;

namespace Client_tech_resversi_api.Migrations
{
    public partial class totp2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorAuth",
                table: "UserAccount",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFactorAuth",
                table: "UserAccount");
        }
    }
}
