using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class AddCommentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsMain",
                table: "Photos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    AutherId = table.Column<string>(type: "TEXT", nullable: true),
                    ActivityId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_AutherId",
                        column: x => x.AutherId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ActivityId",
                table: "Comments",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AutherId",
                table: "Comments",
                column: "AutherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "IsMain",
                table: "Photos",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");
        }
    }
}
