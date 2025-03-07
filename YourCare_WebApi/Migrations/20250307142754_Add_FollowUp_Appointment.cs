using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_WebApi.Migrations
{
    public partial class Add_FollowUp_Appointment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFollowUp",
                table: "Appointment",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousAppointmentID",
                table: "Appointment",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFollowUp",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "PreviousAppointmentID",
                table: "Appointment");
        }
    }
}
