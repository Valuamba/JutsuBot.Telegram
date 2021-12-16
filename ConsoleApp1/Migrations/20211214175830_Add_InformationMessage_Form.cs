using Microsoft.EntityFrameworkCore.Migrations;

namespace ConsoleApp1.Migrations
{
    public partial class Add_InformationMessage_Form : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages",
                column: "InformationMessageFormId",
                unique: true,
                filter: "[InformationMessageFormId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages",
                column: "InformationMessageFormId");
        }
    }
}
