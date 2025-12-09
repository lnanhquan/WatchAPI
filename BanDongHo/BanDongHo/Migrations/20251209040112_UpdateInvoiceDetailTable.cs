using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceDetailTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceDetails_IsDeleted",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "InvoiceDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InvoiceDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "InvoiceDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "InvoiceDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InvoiceDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_IsDeleted",
                table: "InvoiceDetails",
                column: "IsDeleted");
        }
    }
}
