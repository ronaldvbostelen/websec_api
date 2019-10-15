using Microsoft.EntityFrameworkCore.Migrations;

namespace Client_tech_resversi_api.Migrations
{
    public partial class recoverattempts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecoverAttempts",
                table: "UserAccount",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoverAttempts",
                table: "UserAccount");
        }
    }
}
