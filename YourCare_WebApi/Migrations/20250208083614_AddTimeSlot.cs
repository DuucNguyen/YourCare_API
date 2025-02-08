using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_WebApi.Migrations
{
    public partial class AddTimeSlot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "YearExperience",
                table: "DoctorProfile",
                newName: "StartCareerYear");

            migrationBuilder.CreateTable(
                name: "TimeSlot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSlot", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeSlot");

            migrationBuilder.RenameColumn(
                name: "StartCareerYear",
                table: "DoctorProfile",
                newName: "YearExperience");
        }
    }
}
