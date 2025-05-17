using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientTrackingSite.Migrations
{
    /// <inheritdoc />
    public partial class patientdocmed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Users_PatientId",
                table: "Medications");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Medications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medications_DoctorId",
                table: "Medications",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Users_DoctorId",
                table: "Medications",
                column: "DoctorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Users_PatientId",
                table: "Medications",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Users_DoctorId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Users_PatientId",
                table: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_Medications_DoctorId",
                table: "Medications");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Medications");

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Users_PatientId",
                table: "Medications",
                column: "PatientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
