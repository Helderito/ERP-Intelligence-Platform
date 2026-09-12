using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NetDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerm", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Country",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c01"), "AO", "Angola" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c02"), "PT", "Portugal" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c03"), "BR", "Brazil" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c04"), "US", "United States" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c05"), "GB", "United Kingdom" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c06"), "ES", "Spain" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c07"), "FR", "France" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c08"), "DE", "Germany" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c09"), "IT", "Italy" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c10"), "NL", "Netherlands" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c11"), "BE", "Belgium" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c12"), "CH", "Switzerland" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c13"), "LU", "Luxembourg" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c14"), "CV", "Cabo Verde" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c15"), "MZ", "Mozambique" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c16"), "ST", "Sao Tome and Principe" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c17"), "GW", "Guinea-Bissau" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c18"), "TL", "Timor-Leste" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c19"), "ZA", "South Africa" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c20"), "NA", "Namibia" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c21"), "CN", "China" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c22"), "IN", "India" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c23"), "AE", "United Arab Emirates" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c24"), "CA", "Canada" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8c25"), "AU", "Australia" }
                });

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d01"), "AOA", "Angolan Kwanza" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d02"), "EUR", "Euro" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d03"), "USD", "US Dollar" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d04"), "BRL", "Brazilian Real" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d05"), "GBP", "Pound Sterling" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d06"), "CHF", "Swiss Franc" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d07"), "CAD", "Canadian Dollar" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d08"), "AUD", "Australian Dollar" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d09"), "CNY", "Chinese Yuan" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d10"), "INR", "Indian Rupee" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d11"), "ZAR", "South African Rand" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d12"), "NAD", "Namibian Dollar" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d13"), "MZN", "Mozambican Metical" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d14"), "CVE", "Cabo Verde Escudo" },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8d15"), "STN", "Sao Tome and Principe Dobra" }
                });

            migrationBuilder.InsertData(
                table: "PaymentTerm",
                columns: new[] { "Id", "Code", "Name", "NetDays" },
                values: new object[,]
                {
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e01"), "NET0", "Immediate", 0 },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e02"), "NET15", "Net 15 days", 15 },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e03"), "NET30", "Net 30 days", 30 },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e04"), "NET60", "Net 60 days", 60 },
                    { new Guid("7f6a9325-d0a1-4d3b-9d16-9f8a579a8e05"), "NET90", "Net 90 days", 90 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Country_Code",
                table: "Country",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currency_Code",
                table: "Currency",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerm_Code",
                table: "PaymentTerm",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "PaymentTerm");
        }
    }
}
