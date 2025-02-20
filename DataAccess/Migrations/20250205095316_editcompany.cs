using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class editcompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyTrips");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Trips_CompanyId",
                table: "Trips",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Companies_CompanyId",
                table: "Trips",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Companies_CompanyId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_CompanyId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Trips");

            migrationBuilder.CreateTable(
                name: "CompanyTrips",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    TripId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyTrips", x => new { x.CompanyId, x.TripId });
                    table.ForeignKey(
                        name: "FK_CompanyTrips_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyTrips_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyTrips_TripId",
                table: "CompanyTrips",
                column: "TripId");
        }
    }
}
