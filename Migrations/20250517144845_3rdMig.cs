using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientTrackingSite.Migrations
{
    /// <inheritdoc />
    public partial class _3rdMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Users");
        }
    }
}
