using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDriversShiftsAndBreaksTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DriversShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DriverId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriversShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriversShifts_Users_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Breaks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ShiftId = table.Column<Guid>(type: "TEXT", nullable: false),
                    BreakType = table.Column<int>(type: "INTEGER", nullable: false),
                    Start = table.Column<DateTime>(type: "TEXT", nullable: false),
                    End = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Breaks_DriversShifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "DriversShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Breaks_ShiftId",
                table: "Breaks",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_DriversShifts_DriverId",
                table: "DriversShifts",
                column: "DriverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Breaks");

            migrationBuilder.DropTable(
                name: "DriversShifts");
        }
    }
}
