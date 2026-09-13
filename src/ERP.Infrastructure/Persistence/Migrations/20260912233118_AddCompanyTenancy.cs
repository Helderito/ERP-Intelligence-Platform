using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouse_Code",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasure_Code",
                table: "UnitOfMeasure");

            migrationBuilder.DropIndex(
                name: "IX_TaxCode_Code",
                table: "TaxCode");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_Code",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Product_Code",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Code",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Category_Code",
                table: "Category");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Warehouse",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "UnitOfMeasure",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "TaxCode",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Supplier",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Product",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Customer",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Category",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyFiscalProfile",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nif = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    VatRegime = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FiscalAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyFiscalProfile", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_CompanyFiscalProfile_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Establishment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EstablishmentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establishment_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompany_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCompany_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "Id",
                keyValue: new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b01"),
                column: "CompanyId",
                value: new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000001"));

            migrationBuilder.InsertData(
                table: "Company",
                columns: new[] { "Id", "CreatedAtUtc", "DeactivatedAtUtc", "IsActive", "Name", "UpdatedAtUtc" },
                values: new object[] { new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, "Empresa Principal", null });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Code", "Description" },
                values: new object[] { new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000003"), "company.manage", "Manage companies and establishments" });

            migrationBuilder.UpdateData(
                table: "UnitOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b02"),
                column: "CompanyId",
                value: new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000001"));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasure",
                keyColumn: "Id",
                keyValue: new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8b03"),
                column: "CompanyId",
                value: new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000001"));

            migrationBuilder.InsertData(
                table: "Establishment",
                columns: new[] { "Id", "Code", "CompanyId", "EstablishmentNumber", "Name" },
                values: new object[] { new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000002"), "MAIN", new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000001"), "001", "Sede" });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Id", "AssignedAtUtc", "PermissionId", "RoleId" },
                values: new object[] { new Guid("b1a7c0de-0000-4000-a000-000000000108"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000003"), new Guid("b1a7c0de-0000-4000-a000-000000000001") });

            migrationBuilder.Sql("""
                UPDATE "Category" SET "CompanyId" = '5f0a6d53-5e2d-4f65-aeb8-000000000001' WHERE "CompanyId" IS NULL;
                UPDATE "Customer" SET "CompanyId" = '5f0a6d53-5e2d-4f65-aeb8-000000000001' WHERE "CompanyId" IS NULL;
                UPDATE "Product" SET "CompanyId" = '5f0a6d53-5e2d-4f65-aeb8-000000000001' WHERE "CompanyId" IS NULL;
                UPDATE "Supplier" SET "CompanyId" = '5f0a6d53-5e2d-4f65-aeb8-000000000001' WHERE "CompanyId" IS NULL;
                UPDATE "TaxCode" SET "CompanyId" = '5f0a6d53-5e2d-4f65-aeb8-000000000001' WHERE "CompanyId" IS NULL;
                UPDATE "UnitOfMeasure" SET "CompanyId" = '5f0a6d53-5e2d-4f65-aeb8-000000000001' WHERE "CompanyId" IS NULL;
                UPDATE "Warehouse" SET "CompanyId" = '5f0a6d53-5e2d-4f65-aeb8-000000000001' WHERE "CompanyId" IS NULL;

                INSERT INTO "UserCompany" ("Id", "UserId", "CompanyId", "AssignedAtUtc")
                SELECT md5("Id"::text || '5f0a6d53-5e2d-4f65-aeb8-000000000001')::uuid,
                       "Id",
                       '5f0a6d53-5e2d-4f65-aeb8-000000000001',
                       TIMESTAMPTZ '2026-01-01 00:00:00+00'
                FROM "User"
                WHERE NOT EXISTS (
                    SELECT 1 FROM "UserCompany" membership WHERE membership."UserId" = "User"."Id");
                """);

            foreach (var table in new[]
                     {
                         "Category", "Customer", "Product", "Supplier", "TaxCode", "UnitOfMeasure", "Warehouse"
                     })
            {
                migrationBuilder.AlterColumn<Guid>(
                    name: "CompanyId",
                    table: table,
                    type: "uuid",
                    nullable: false,
                    oldClrType: typeof(Guid),
                    oldType: "uuid",
                    oldNullable: true);
            }

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_CompanyId",
                table: "Warehouse",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_CompanyId_Code",
                table: "Warehouse",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasure_CompanyId",
                table: "UnitOfMeasure",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasure_CompanyId_Code",
                table: "UnitOfMeasure",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxCode_CompanyId",
                table: "TaxCode",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxCode_CompanyId_Code",
                table: "TaxCode",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_CompanyId",
                table: "Supplier",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_CompanyId_Code",
                table: "Supplier",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_CompanyId",
                table: "Product",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CompanyId_Code",
                table: "Product",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CompanyId",
                table: "Customer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CompanyId_Code",
                table: "Customer",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId",
                table: "Category",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_CompanyId_Code",
                table: "Category",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Establishment_CompanyId_Code",
                table: "Establishment",
                columns: new[] { "CompanyId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCompany_CompanyId",
                table: "UserCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompany_UserId",
                table: "UserCompany",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Company_CompanyId",
                table: "Category",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Customer_Company_CompanyId",
                table: "Customer",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Company_CompanyId",
                table: "Product",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Supplier_Company_CompanyId",
                table: "Supplier",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaxCode_Company_CompanyId",
                table: "TaxCode",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UnitOfMeasure_Company_CompanyId",
                table: "UnitOfMeasure",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouse_Company_CompanyId",
                table: "Warehouse",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Company_CompanyId",
                table: "Category");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Company_CompanyId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Company_CompanyId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_Supplier_Company_CompanyId",
                table: "Supplier");

            migrationBuilder.DropForeignKey(
                name: "FK_TaxCode_Company_CompanyId",
                table: "TaxCode");

            migrationBuilder.DropForeignKey(
                name: "FK_UnitOfMeasure_Company_CompanyId",
                table: "UnitOfMeasure");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_Company_CompanyId",
                table: "Warehouse");

            migrationBuilder.DropTable(
                name: "CompanyFiscalProfile");

            migrationBuilder.DropTable(
                name: "Establishment");

            migrationBuilder.DropTable(
                name: "UserCompany");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_CompanyId",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_CompanyId_Code",
                table: "Warehouse");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasure_CompanyId",
                table: "UnitOfMeasure");

            migrationBuilder.DropIndex(
                name: "IX_UnitOfMeasure_CompanyId_Code",
                table: "UnitOfMeasure");

            migrationBuilder.DropIndex(
                name: "IX_TaxCode_CompanyId",
                table: "TaxCode");

            migrationBuilder.DropIndex(
                name: "IX_TaxCode_CompanyId_Code",
                table: "TaxCode");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_CompanyId",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_CompanyId_Code",
                table: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_Product_CompanyId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_CompanyId_Code",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Customer_CompanyId",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Customer_CompanyId_Code",
                table: "Customer");

            migrationBuilder.DropIndex(
                name: "IX_Category_CompanyId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_CompanyId_Code",
                table: "Category");

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("b1a7c0de-0000-4000-a000-000000000108"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5f0a6d53-5e2d-4f65-aeb8-000000000003"));

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "UnitOfMeasure");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "TaxCode");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Supplier");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_Code",
                table: "Warehouse",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasure_Code",
                table: "UnitOfMeasure",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxCode_Code",
                table: "TaxCode",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Code",
                table: "Supplier",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Product_Code",
                table: "Product",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Code",
                table: "Customer",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Code",
                table: "Category",
                column: "Code",
                unique: true);
        }
    }
}
