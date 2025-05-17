using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientTrackingSite.Migrations
{
    /// <inheritdoc />
    public partial class tcno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TCNo",
                table: "Users",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TCNo",
                table: "Users",
                column: "TCNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_TCNo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TCNo",
                table: "Users");
        }
    }
}
