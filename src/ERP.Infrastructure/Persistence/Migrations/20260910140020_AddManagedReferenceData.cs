using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddManagedReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "UnitOfMeasure",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAtUtc",
                table: "UnitOfMeasure",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UnitOfMeasure",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "UnitOfMeasure",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Category",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAtUtc",
                table: "Category",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Category",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Category",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaxCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxCode", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b01"),
                columns: new[] { "CreatedAtUtc", "DeactivatedAtUtc", "IsActive", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[] { new Guid("cd69240d-0864-4a4c-a6a3-a159b831812e"), "reference.manage", "Manage shared reference data" });

            migrationBuilder.UpdateData(
                table: "UnitOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b02"),
                columns: new[] { "CreatedAtUtc", "DeactivatedAtUtc", "IsActive", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null });

            migrationBuilder.UpdateData(
                table: "UnitOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b03"),
                columns: new[] { "CreatedAtUtc", "DeactivatedAtUtc", "IsActive", "UpdatedAtUtc" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Id", "AssignedAtUtc", "PermissionId", "RoleId" },
                values: new object[] { new Guid("b1a7c0de-0000-4000-a000-000000000107"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("cd69240d-0864-4a4c-a6a3-a159b831812e"), new Guid("b1a7c0de-0000-4000-a000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "IX_TaxCode_Code",
                table: "TaxCode",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxCode");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("b1a7c0de-0000-4000-a000-000000000107"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("cd69240d-0864-4a4c-a6a3-a159b831812e"));

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "UnitOfMeasure");

            migrationBuilder.DropColumn(
                name: "DeactivatedAtUtc",
                table: "UnitOfMeasure");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UnitOfMeasure");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "UnitOfMeasure");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "DeactivatedAtUtc",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Category");
        }
    }
}
