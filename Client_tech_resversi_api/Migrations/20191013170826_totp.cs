using Microsoft.EntityFrameworkCore.Migrations;

namespace Client_tech_resversi_api.Migrations
{
    public partial class totp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TotpHash",
                table: "UserAccount",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TotpSecret",
                table: "UserAccount",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotpHash",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "TotpSecret",
                table: "UserAccount");
        }
    }
}
