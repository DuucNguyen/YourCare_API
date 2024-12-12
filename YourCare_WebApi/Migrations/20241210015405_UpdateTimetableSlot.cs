using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_WebApi.Migrations
{
    public partial class UpdateTimetableSlot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialty_Specialty",
                table: "DoctorSpecialization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialization",
                table: "Specialization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorSpecialization",
                table: "DoctorSpecialization");

            migrationBuilder.RenameTable(
                name: "Specialization",
                newName: "Specialty");

            migrationBuilder.RenameTable(
                name: "DoctorSpecialization",
                newName: "DoctorSpecialties");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorSpecialization_SpecialtyID",
                table: "DoctorSpecialties",
                newName: "IX_DoctorSpecialties_SpecialtyID");

            migrationBuilder.AddColumn<int>(
                name: "AvailableSLots",
                table: "Timetable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TimetableSlot",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialty",
                table: "Specialty",
                column: "SpecialtyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorSpecialties",
                table: "DoctorSpecialties",
                columns: new[] { "DoctorID", "SpecialtyID" });

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialties_Specialty",
                table: "DoctorSpecialties",
                column: "SpecialtyID",
                principalTable: "Specialty",
                principalColumn: "SpecialtyID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecialties_Specialty",
                table: "DoctorSpecialties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Specialty",
                table: "Specialty");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DoctorSpecialties",
                table: "DoctorSpecialties");

            migrationBuilder.DropColumn(
                name: "AvailableSLots",
                table: "Timetable");

            migrationBuilder.DropColumn(
                name: "TimetableSlot",
                table: "Appointment");

            migrationBuilder.RenameTable(
                name: "Specialty",
                newName: "Specialization");

            migrationBuilder.RenameTable(
                name: "DoctorSpecialties",
                newName: "DoctorSpecialization");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorSpecialties_SpecialtyID",
                table: "DoctorSpecialization",
                newName: "IX_DoctorSpecialization_SpecialtyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Specialization",
                table: "Specialization",
                column: "SpecialtyID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DoctorSpecialization",
                table: "DoctorSpecialization",
                columns: new[] { "DoctorID", "SpecialtyID" });

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecialty_Specialty",
                table: "DoctorSpecialization",
                column: "SpecialtyID",
                principalTable: "Specialization",
                principalColumn: "SpecialtyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
