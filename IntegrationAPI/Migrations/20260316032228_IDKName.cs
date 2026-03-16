using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntegrationAPI.Migrations
{
    /// <inheritdoc />
    public partial class IDKName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Confirmed",
                table: "Shippings",
                newName: "ShippingStatus");

            migrationBuilder.AddColumn<double>(
                name: "BodySize_Height",
                table: "Vehicles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BodySize_Length",
                table: "Vehicles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BodySize_Width",
                table: "Vehicles",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "HireDate",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "Users",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentDate",
                table: "ShippingOrders",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivedDate",
                table: "ShippingOrders",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverFio",
                table: "ShippingOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ShippingOrders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DriversLicense",
                table: "Drivers",
                type: "TEXT",
                maxLength: 40,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodySize_Height",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "BodySize_Length",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "BodySize_Width",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "HireDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReceiverFio",
                table: "ShippingOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShippingOrders");

            migrationBuilder.DropColumn(
                name: "DriversLicense",
                table: "Drivers");

            migrationBuilder.RenameColumn(
                name: "ShippingStatus",
                table: "Shippings",
                newName: "Confirmed");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentDate",
                table: "ShippingOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivedDate",
                table: "ShippingOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
