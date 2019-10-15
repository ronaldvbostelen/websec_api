using Microsoft.EntityFrameworkCore.Migrations;

namespace Client_tech_resversi_api.Migrations
{
    public partial class hash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SecurityHash",
                table: "UserAccount",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Salt",
                table: "User",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityHash",
                table: "UserAccount");

            migrationBuilder.AlterColumn<string>(
                name: "Salt",
                table: "User",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
