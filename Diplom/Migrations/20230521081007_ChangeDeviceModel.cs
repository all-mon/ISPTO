using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Diplom.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeviceModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Analogue",
                table: "Device");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID",
                table: "Device",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Device_DeviceID",
                table: "Device",
                column: "DeviceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Device_Device_DeviceID",
                table: "Device",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Device_Device_DeviceID",
                table: "Device");

            migrationBuilder.DropIndex(
                name: "IX_Device_DeviceID",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "DeviceID",
                table: "Device");

            migrationBuilder.AddColumn<string>(
                name: "Analogue",
                table: "Device",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
