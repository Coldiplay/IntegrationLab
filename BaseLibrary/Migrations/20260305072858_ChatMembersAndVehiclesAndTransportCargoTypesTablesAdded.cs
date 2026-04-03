using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChatMembersAndVehiclesAndTransportCargoTypesTablesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "Shippings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChatMembers",
                columns: table => new
                {
                    ChatId = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_ChatMembers_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMembers_Users_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    BrandName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Model = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    LiftingCapacity = table.Column<float>(type: "REAL", nullable: false),
                    BodyType = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxCargoVolume = table.Column<float>(type: "REAL", nullable: false),
                    FuelType = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxFuelVolume = table.Column<float>(type: "REAL", nullable: false),
                    VehicleWeight = table.Column<float>(type: "REAL", nullable: false),
                    NumberOfAxes = table.Column<byte>(type: "INTEGER", nullable: false),
                    VehicleSize_Height = table.Column<double>(type: "REAL", nullable: false),
                    VehicleSize_Length = table.Column<double>(type: "REAL", nullable: false),
                    VehicleSize_Width = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportCargoTypes",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false),
                    CargoTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_TransportCargoTypes_CargoTypes_CargoTypeId",
                        column: x => x.CargoTypeId,
                        principalTable: "CargoTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransportCargoTypes_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_VehicleId",
                table: "Shippings",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_ChatId",
                table: "ChatMembers",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMembers_MemberId",
                table: "ChatMembers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportCargoTypes_CargoTypeId",
                table: "TransportCargoTypes",
                column: "CargoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportCargoTypes_VehicleId",
                table: "TransportCargoTypes",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_Vehicles_VehicleId",
                table: "Shippings",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_Vehicles_VehicleId",
                table: "Shippings");

            migrationBuilder.DropTable(
                name: "ChatMembers");

            migrationBuilder.DropTable(
                name: "TransportCargoTypes");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Shippings_VehicleId",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Shippings");
        }
    }
}
