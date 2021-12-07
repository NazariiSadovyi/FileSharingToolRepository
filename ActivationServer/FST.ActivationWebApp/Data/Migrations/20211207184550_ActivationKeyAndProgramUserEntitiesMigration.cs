using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FST.ActivationWebApp.Data.Migrations
{
    public partial class ActivationKeyAndProgramUserEntitiesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    MachineId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivationKey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Key = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivationKey", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivationKey_ProgramUser_Id",
                        column: x => x.Id,
                        principalTable: "ProgramUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivationKey");

            migrationBuilder.DropTable(
                name: "ProgramUser");
        }
    }
}
