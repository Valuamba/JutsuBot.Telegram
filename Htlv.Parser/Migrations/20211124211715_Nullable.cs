using Microsoft.EntityFrameworkCore.Migrations;

namespace Htlv.Parser.Migrations
{
    public partial class Nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_States_CurrentStateUserId",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_States_PrevStateUserId",
                table: "States");

            migrationBuilder.AlterColumn<long>(
                name: "PrevStateUserId",
                table: "States",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "MessageStateUserId",
                table: "States",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "CurrentStateUserId",
                table: "States",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_States_CurrentStateUserId",
                table: "States",
                column: "CurrentStateUserId",
                unique: true,
                filter: "[CurrentStateUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_States_PrevStateUserId",
                table: "States",
                column: "PrevStateUserId",
                unique: true,
                filter: "[PrevStateUserId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_States_CurrentStateUserId",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_States_PrevStateUserId",
                table: "States");

            migrationBuilder.AlterColumn<long>(
                name: "PrevStateUserId",
                table: "States",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "MessageStateUserId",
                table: "States",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "CurrentStateUserId",
                table: "States",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_CurrentStateUserId",
                table: "States",
                column: "CurrentStateUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_PrevStateUserId",
                table: "States",
                column: "PrevStateUserId",
                unique: true);
        }
    }
}
