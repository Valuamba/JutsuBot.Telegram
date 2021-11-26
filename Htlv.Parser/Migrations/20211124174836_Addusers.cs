using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Htlv.Parser.Migrations
{
    public partial class Addusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CSGOMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatchMeta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstTeam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondTeam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MatchEvent = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CSGOMatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAuthorized = table.Column<bool>(type: "bit", nullable: false),
                    IsBotStopped = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CacheData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Step = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    CurrentStateUserId = table.Column<long>(type: "bigint", nullable: false),
                    PrevStateUserId = table.Column<long>(type: "bigint", nullable: false),
                    MessageStateUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.StateId);
                    table.ForeignKey(
                        name: "FK_States_Users_CurrentStateUserId",
                        column: x => x.CurrentStateUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_States_Users_MessageStateUserId",
                        column: x => x.MessageStateUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_States_Users_PrevStateUserId",
                        column: x => x.PrevStateUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_States_CurrentStateUserId",
                table: "States",
                column: "CurrentStateUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_MessageStateUserId",
                table: "States",
                column: "MessageStateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_States_PrevStateUserId",
                table: "States",
                column: "PrevStateUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CSGOMatches");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
