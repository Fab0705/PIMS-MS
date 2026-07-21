using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PIMS_MS.Modules.Inventory.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPeruvianRegions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WorkOrderNumber",
                schema: "inventory",
                table: "WorkOrders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "inventory",
                table: "WorkOrders",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "inventory",
                table: "WorkOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "inventory",
                table: "WorkOrders",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "inventory",
                table: "WorkOrders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "inventory",
                table: "Stocks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "inventory",
                table: "Stocks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNumber",
                schema: "inventory",
                table: "SpareParts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "inventory",
                table: "SpareParts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRework",
                schema: "inventory",
                table: "SpareParts",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "inventory",
                table: "SpareParts",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "inventory",
                table: "SpareParts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "inventory",
                table: "Regions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "inventory",
                table: "Regions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "inventory",
                table: "Locations",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                schema: "inventory",
                table: "Regions",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "AMA", "Amazonas" },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "ANC", "Áncash" },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), "APU", "Apurímac" },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), "ARE", "Arequipa" },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), "AYA", "Ayacucho" },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), "CAJ", "Cajamarca" },
                    { new Guid("a0000000-0000-0000-0000-000000000007"), "CAL", "Callao" },
                    { new Guid("a0000000-0000-0000-0000-000000000008"), "CUS", "Cusco" },
                    { new Guid("a0000000-0000-0000-0000-000000000009"), "HUV", "Huancavelica" },
                    { new Guid("a0000000-0000-0000-0000-000000000010"), "HUC", "Huánuco" },
                    { new Guid("a0000000-0000-0000-0000-000000000011"), "ICA", "Ica" },
                    { new Guid("a0000000-0000-0000-0000-000000000012"), "JUN", "Junín" },
                    { new Guid("a0000000-0000-0000-0000-000000000013"), "LAL", "La Libertad" },
                    { new Guid("a0000000-0000-0000-0000-000000000014"), "LAM", "Lambayeque" },
                    { new Guid("a0000000-0000-0000-0000-000000000016"), "LIP", "Lima Provincias" },
                    { new Guid("a0000000-0000-0000-0000-000000000017"), "LOR", "Loreto" },
                    { new Guid("a0000000-0000-0000-0000-000000000018"), "MDD", "Madre de Dios" },
                    { new Guid("a0000000-0000-0000-0000-000000000019"), "MOQ", "Moquegua" },
                    { new Guid("a0000000-0000-0000-0000-000000000020"), "PAS", "Pasco" },
                    { new Guid("a0000000-0000-0000-0000-000000000021"), "PIU", "Piura" },
                    { new Guid("a0000000-0000-0000-0000-000000000022"), "PUN", "Puno" },
                    { new Guid("a0000000-0000-0000-0000-000000000023"), "SAM", "San Martín" },
                    { new Guid("a0000000-0000-0000-0000-000000000024"), "TAC", "Tacna" },
                    { new Guid("a0000000-0000-0000-0000-000000000025"), "TUM", "Tumbes" },
                    { new Guid("a0000000-0000-0000-0000-000000000026"), "UCA", "Ucayali" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_WorkOrderNumber",
                schema: "inventory",
                table: "WorkOrders",
                column: "WorkOrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpareParts_PartNumber",
                schema: "inventory",
                table: "SpareParts",
                column: "PartNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Code",
                schema: "inventory",
                table: "Regions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Regions_Name",
                schema: "inventory",
                table: "Regions",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_WorkOrderNumber",
                schema: "inventory",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_SpareParts_PartNumber",
                schema: "inventory",
                table: "SpareParts");

            migrationBuilder.DropIndex(
                name: "IX_Regions_Code",
                schema: "inventory",
                table: "Regions");

            migrationBuilder.DropIndex(
                name: "IX_Regions_Name",
                schema: "inventory",
                table: "Regions");

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                schema: "inventory",
                table: "Regions",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000026"));

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "inventory",
                table: "Regions");

            migrationBuilder.AlterColumn<string>(
                name: "WorkOrderNumber",
                schema: "inventory",
                table: "WorkOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "inventory",
                table: "WorkOrders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "inventory",
                table: "WorkOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "inventory",
                table: "WorkOrders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "inventory",
                table: "WorkOrders",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "inventory",
                table: "Stocks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "inventory",
                table: "Stocks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartNumber",
                schema: "inventory",
                table: "SpareParts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                schema: "inventory",
                table: "SpareParts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRework",
                schema: "inventory",
                table: "SpareParts",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "inventory",
                table: "SpareParts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "inventory",
                table: "SpareParts",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "inventory",
                table: "Regions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "inventory",
                table: "Locations",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);
        }
    }
}
