using Microsoft.EntityFrameworkCore.Migrations;

namespace ConsoleApp1.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormCacheModel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                });

            migrationBuilder.CreateTable(
                name: "LocalizationMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizationMessages", x => x.Id);
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
                name: "FormProperties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PropertyStatus = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangePropertyTextAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPropertyValueTextAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceholderAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddPropertyCommandAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangePropertyCommandAlias = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormProperties_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "FormId");
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
                    StatePriority = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    CurrentStateUserId = table.Column<long>(type: "bigint", nullable: true),
                    MessageStateUserId = table.Column<long>(type: "bigint", nullable: true)
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
                        name: "FK_States_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrackedMessages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ChatId = table.Column<long>(type: "bigint", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    InformationMessageFormId = table.Column<int>(type: "int", nullable: true),
                    UtilityMessageFormId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedMessages", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_TrackedMessages_Forms_InformationMessageFormId",
                        column: x => x.InformationMessageFormId,
                        principalTable: "Forms",
                        principalColumn: "FormId");
                    table.ForeignKey(
                        name: "FK_TrackedMessages_Forms_UtilityMessageFormId",
                        column: x => x.UtilityMessageFormId,
                        principalTable: "Forms",
                        principalColumn: "FormId");
                    table.ForeignKey(
                        name: "FK_TrackedMessages_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "StateId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormProperties_FormId",
                table: "FormProperties",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_States_CurrentStateUserId",
                table: "States",
                column: "CurrentStateUserId",
                unique: true,
                filter: "[CurrentStateUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_States_MessageStateUserId",
                table: "States",
                column: "MessageStateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_States_UserId",
                table: "States",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedMessages_InformationMessageFormId",
                table: "TrackedMessages",
                column: "InformationMessageFormId",
                unique: true,
                filter: "[InformationMessageFormId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedMessages_StateId",
                table: "TrackedMessages",
                column: "StateId",
                unique: true,
                filter: "[StateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedMessages_UtilityMessageFormId",
                table: "TrackedMessages",
                column: "UtilityMessageFormId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormProperties");

            migrationBuilder.DropTable(
                name: "LocalizationMessages");

            migrationBuilder.DropTable(
                name: "TrackedMessages");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
