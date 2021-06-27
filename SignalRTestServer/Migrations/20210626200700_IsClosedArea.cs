using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalRTestServer.Migrations
{
    public partial class IsClosedArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Areas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "AreaPoints",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Areas");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "AreaPoints");
        }
    }
}
