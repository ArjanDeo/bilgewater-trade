using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BilgewaterTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSnapshotsAndListings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuctionHouseSnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Realm = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "text", nullable: false),
                    FetchedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionHouseSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommodityListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    AuctionHouseSnapshotId = table.Column<int>(type: "integer", nullable: false),
                    UnitPriceCopper = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TimeLeft = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommodityListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommodityListings_AuctionHouseSnapshots_AuctionHouseSnapsho~",
                        column: x => x.AuctionHouseSnapshotId,
                        principalTable: "AuctionHouseSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommodityListings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RealmListings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    AuctionHouseSnapshotId = table.Column<int>(type: "integer", nullable: false),
                    BuyoutCopper = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    TimeLeft = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RealmListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RealmListings_AuctionHouseSnapshots_AuctionHouseSnapshotId",
                        column: x => x.AuctionHouseSnapshotId,
                        principalTable: "AuctionHouseSnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RealmListings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommodityListings_AuctionHouseSnapshotId",
                table: "CommodityListings",
                column: "AuctionHouseSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_CommodityListings_ItemId",
                table: "CommodityListings",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RealmListings_AuctionHouseSnapshotId",
                table: "RealmListings",
                column: "AuctionHouseSnapshotId");

            migrationBuilder.CreateIndex(
                name: "IX_RealmListings_ItemId",
                table: "RealmListings",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommodityListings");

            migrationBuilder.DropTable(
                name: "RealmListings");

            migrationBuilder.DropTable(
                name: "AuctionHouseSnapshots");
        }
    }
}
