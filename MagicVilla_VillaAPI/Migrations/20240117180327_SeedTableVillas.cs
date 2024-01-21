using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVillaVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedTableVillas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenity", "CreatedDate", "Details", "ImageUrl", "Name", "Occupancy", "Rate", "Sqft", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, "", new DateTime(2024, 1, 18, 1, 3, 27, 63, DateTimeKind.Local).AddTicks(8320), "The best you cant get!", "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA1n6ceK.img?w=1920&h=1080&q=60&m=2&f=jpg", "Royal Villa", 5, 5.0, 50, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "yokk", new DateTime(2024, 1, 18, 1, 3, 27, 63, DateTimeKind.Local).AddTicks(8330), "The best you cant get!", "https://img-s-msn-com.akamaized.net/tenant/amp/entityid/AA1n6ceK.img?w=1920&h=1080&q=60&m=2&f=jpg", "Royal Villa second", 4, 4.0, 40, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
