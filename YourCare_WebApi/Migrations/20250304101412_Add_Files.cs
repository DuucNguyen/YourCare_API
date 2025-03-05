using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourCare_WebApi.Migrations
{
    public partial class Add_Files : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentFilesUpload",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentID = table.Column<int>(type: "int", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentFilesUpload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_AppointmentFilesUpload",
                        column: x => x.AppointmentID,
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentFilesUpload_AppointmentID",
                table: "AppointmentFilesUpload",
                column: "AppointmentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentFilesUpload");
        }
    }
}
