using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_WebApi.Migrations
{
    public partial class Remove_TimeSlot_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TimeTable_TimeSlot",
                table: "Timetable");

            migrationBuilder.DropIndex(
                name: "IX_Timetable_DoctorID_Date_TimeSlotID",
                table: "Timetable");

            migrationBuilder.DropIndex(
                name: "IX_Timetable_TimeSlotID",
                table: "Timetable");

            migrationBuilder.DropColumn(
                name: "TimeSlotID",
                table: "Timetable");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Timetable",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Timetable",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Timetable_DoctorID_Date_StartTime_EndTime",
                table: "Timetable",
                columns: new[] { "DoctorID", "Date", "StartTime", "EndTime" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timetable_DoctorID_Date_StartTime_EndTime",
                table: "Timetable");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Timetable");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Timetable");

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotID",
                table: "Timetable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Timetable_DoctorID_Date_TimeSlotID",
                table: "Timetable",
                columns: new[] { "DoctorID", "Date", "TimeSlotID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timetable_TimeSlotID",
                table: "Timetable",
                column: "TimeSlotID");

            migrationBuilder.AddForeignKey(
                name: "FK_TimeTable_TimeSlot",
                table: "Timetable",
                column: "TimeSlotID",
                principalTable: "TimeSlot",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
