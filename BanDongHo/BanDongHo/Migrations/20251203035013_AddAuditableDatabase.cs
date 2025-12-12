using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WatchAPI.Migrations;

/// <inheritdoc />
public partial class AddAuditableDatabase : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "ImageUrl",
            table: "Watches",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "Watches",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Watches",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DeletedAt",
            table: "Watches",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            table: "Watches",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Watches",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "Watches",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "Watches",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Version",
            table: "Watches",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "Invoices",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DeletedAt",
            table: "Invoices",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            table: "Invoices",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "Invoices",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "Invoices",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "Invoices",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Version",
            table: "Invoices",
            type: "int",
            nullable: false,
            defaultValue: 0);

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

        migrationBuilder.AddColumn<int>(
            name: "Version",
            table: "InvoiceDetails",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "CartItems",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "CartItems",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DeletedAt",
            table: "CartItems",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            table: "CartItems",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "CartItems",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "CartItems",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "CartItems",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Version",
            table: "CartItems",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAt",
            table: "AspNetUsers",
            type: "datetime2",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "CreatedBy",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DeletedAt",
            table: "AspNetUsers",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DeletedBy",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            table: "AspNetUsers",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "AspNetUsers",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "UpdatedBy",
            table: "AspNetUsers",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Version",
            table: "AspNetUsers",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateIndex(
            name: "IX_Watches_IsDeleted",
            table: "Watches",
            column: "IsDeleted");

        migrationBuilder.CreateIndex(
            name: "IX_Invoices_IsDeleted",
            table: "Invoices",
            column: "IsDeleted");

        migrationBuilder.CreateIndex(
            name: "IX_InvoiceDetails_IsDeleted",
            table: "InvoiceDetails",
            column: "IsDeleted");

        migrationBuilder.CreateIndex(
            name: "IX_CartItems_IsDeleted",
            table: "CartItems",
            column: "IsDeleted");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Watches_IsDeleted",
            table: "Watches");

        migrationBuilder.DropIndex(
            name: "IX_Invoices_IsDeleted",
            table: "Invoices");

        migrationBuilder.DropIndex(
            name: "IX_InvoiceDetails_IsDeleted",
            table: "InvoiceDetails");

        migrationBuilder.DropIndex(
            name: "IX_CartItems_IsDeleted",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "DeletedAt",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "DeletedBy",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "Version",
            table: "Watches");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "Invoices");

        migrationBuilder.DropColumn(
            name: "DeletedAt",
            table: "Invoices");

        migrationBuilder.DropColumn(
            name: "DeletedBy",
            table: "Invoices");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "Invoices");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "Invoices");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "Invoices");

        migrationBuilder.DropColumn(
            name: "Version",
            table: "Invoices");

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

        migrationBuilder.DropColumn(
            name: "Version",
            table: "InvoiceDetails");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "DeletedAt",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "DeletedBy",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "Version",
            table: "CartItems");

        migrationBuilder.DropColumn(
            name: "CreatedAt",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "CreatedBy",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "DeletedAt",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "DeletedBy",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "IsDeleted",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "UpdatedBy",
            table: "AspNetUsers");

        migrationBuilder.DropColumn(
            name: "Version",
            table: "AspNetUsers");

        migrationBuilder.AlterColumn<string>(
            name: "ImageUrl",
            table: "Watches",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);
    }
}
