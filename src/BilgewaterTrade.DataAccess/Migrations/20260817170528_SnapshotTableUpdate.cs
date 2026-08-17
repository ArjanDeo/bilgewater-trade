using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilgewaterTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Realm",
                table: "AuctionHouseSnapshots");

            migrationBuilder.AddColumn<int>(
                name: "ConnectedRealmId",
                table: "AuctionHouseSnapshots",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectedRealmId",
                table: "AuctionHouseSnapshots");

            migrationBuilder.AddColumn<string>(
                name: "Realm",
                table: "AuctionHouseSnapshots",
                type: "text",
                nullable: true);
        }
    }
}
