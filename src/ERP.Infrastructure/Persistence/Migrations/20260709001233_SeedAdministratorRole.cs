using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdministratorRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedAtUtc", "DeactivatedAtUtc", "IsActive", "Name", "UpdatedAtUtc" },
                values: new object[] { new Guid("b1a7c0de-0000-4000-a000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Administrator", null });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Id", "AssignedAtUtc", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("b1a7c0de-0000-4000-a000-000000000101"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6fe633a8-dbed-4dc1-a572-f0e42f7d0d3a"), new Guid("b1a7c0de-0000-4000-a000-000000000001") },
                    { new Guid("b1a7c0de-0000-4000-a000-000000000102"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("97388676-8f9c-4f9c-a2f1-9c972dd4c19d"), new Guid("b1a7c0de-0000-4000-a000-000000000001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("b1a7c0de-0000-4000-a000-000000000101"));

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("b1a7c0de-0000-4000-a000-000000000102"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("b1a7c0de-0000-4000-a000-000000000001"));
        }
    }
}
