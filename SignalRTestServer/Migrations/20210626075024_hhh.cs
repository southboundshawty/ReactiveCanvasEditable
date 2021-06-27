using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalRTestServer.Migrations
{
    public partial class hhh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AreaId1",
                table: "AreaPoints",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AreaPoints_AreaId1",
                table: "AreaPoints",
                column: "AreaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AreaPoints_Areas_AreaId1",
                table: "AreaPoints",
                column: "AreaId1",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AreaPoints_Areas_AreaId1",
                table: "AreaPoints");

            migrationBuilder.DropIndex(
                name: "IX_AreaPoints_AreaId1",
                table: "AreaPoints");

            migrationBuilder.DropColumn(
                name: "AreaId1",
                table: "AreaPoints");
        }
    }
}
