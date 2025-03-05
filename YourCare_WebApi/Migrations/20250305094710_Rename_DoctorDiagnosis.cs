using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_WebApi.Migrations
{
    public partial class Rename_DoctorDiagnosis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DoctorDianosis",
                table: "Appointment",
                newName: "DoctorDiagnosis");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DoctorDiagnosis",
                table: "Appointment",
                newName: "DoctorDianosis");
        }
    }
}
