using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PIMS_MS.Modules.Logistics.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialLogistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "logistics");

            migrationBuilder.CreateTable(
                name: "Replenishments",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replenishments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transfers",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OriginLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ExceptionNotes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReplenishmentItems",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReplenishmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    SparePartId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplenishmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReplenishmentItems_Replenishments_ReplenishmentId",
                        column: x => x.ReplenishmentId,
                        principalSchema: "logistics",
                        principalTable: "Replenishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferItems",
                schema: "logistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    SparePartId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferItems_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalSchema: "logistics",
                        principalTable: "Transfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentItems_ReplenishmentId",
                schema: "logistics",
                table: "ReplenishmentItems",
                column: "ReplenishmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReplenishmentItems_SparePartId",
                schema: "logistics",
                table: "ReplenishmentItems",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_Replenishments_LocationId",
                schema: "logistics",
                table: "Replenishments",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_SparePartId",
                schema: "logistics",
                table: "TransferItems",
                column: "SparePartId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferItems_TransferId",
                schema: "logistics",
                table: "TransferItems",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_DestinationLocationId",
                schema: "logistics",
                table: "Transfers",
                column: "DestinationLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_OriginLocationId",
                schema: "logistics",
                table: "Transfers",
                column: "OriginLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_TrackingCode",
                schema: "logistics",
                table: "Transfers",
                column: "TrackingCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReplenishmentItems",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "TransferItems",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "Replenishments",
                schema: "logistics");

            migrationBuilder.DropTable(
                name: "Transfers",
                schema: "logistics");
        }
    }
}
