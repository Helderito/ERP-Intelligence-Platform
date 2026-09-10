using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WarehouseType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WarehouseTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouse_WarehouseType_WarehouseTypeId",
                        column: x => x.WarehouseTypeId,
                        principalTable: "WarehouseType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[] { new Guid("74e9f7f1-63d6-4c29-b253-5d02ac4dd878"), "warehouses.manage", "Manage warehouses" });

            migrationBuilder.InsertData(
                table: "WarehouseType",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b04"), "MAIN", "Main Warehouse" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b05"), "TRANSIT", "Transit" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b06"), "VIRTUAL", "Virtual" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Id", "AssignedAtUtc", "PermissionId", "RoleId" },
                values: new object[] { new Guid("b1a7c0de-0000-4000-a000-000000000106"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("74e9f7f1-63d6-4c29-b253-5d02ac4dd878"), new Guid("b1a7c0de-0000-4000-a000-000000000001") });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Code",
                table: "Warehouse",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_WarehouseTypeId",
                table: "Warehouse",
                column: "WarehouseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseType_Code",
                table: "WarehouseType",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "WarehouseType");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("b1a7c0de-0000-4000-a000-000000000106"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("74e9f7f1-63d6-4c29-b253-5d02ac4dd878"));
        }
    }
}
