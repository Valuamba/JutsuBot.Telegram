using Microsoft.EntityFrameworkCore.Migrations;

namespace ConsoleApp1.Migrations
{
    public partial class Forms_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormProperties_Forms_FormId",
                table: "FormProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackedMessages_Forms_InformationMessageFormId",
                table: "TrackedMessages");

            migrationBuilder.DropIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages");

            migrationBuilder.AddColumn<long>(
                name: "ChatId",
                table: "Forms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Forms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages",
                column: "InformationMessageFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_UserId",
                table: "Forms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormProperties_Forms_FormId",
                table: "FormProperties",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "FormId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Forms_Users_UserId",
                table: "Forms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedMessages_Forms_InformationMessageFormId",
                table: "TrackedMessages",
                column: "InformationMessageFormId",
                principalTable: "Forms",
                principalColumn: "FormId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormProperties_Forms_FormId",
                table: "FormProperties");

            migrationBuilder.DropForeignKey(
                name: "FK_Forms_Users_UserId",
                table: "Forms");

            migrationBuilder.DropForeignKey(
                name: "FK_TrackedMessages_Forms_InformationMessageFormId",
                table: "TrackedMessages");

            migrationBuilder.DropIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages");

            migrationBuilder.DropIndex(
                name: "IX_Forms_UserId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Forms");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages",
                column: "InformationMessageFormId",
                unique: true,
                filter: "[InformationMessageFormId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_FormProperties_Forms_FormId",
                table: "FormProperties",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackedMessages_Forms_InformationMessageFormId",
                table: "TrackedMessages",
                column: "InformationMessageFormId",
                principalTable: "Forms",
                principalColumn: "FormId");
        }
    }
}
