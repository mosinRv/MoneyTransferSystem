using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneyTransferSystem.Migrations
{
    public partial class Currency__MaxTransferSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "is_approved",
                table: "transfers",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "max_transfer_size",
                table: "currencies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "max_transfer_size",
                table: "currencies");

            migrationBuilder.AlterColumn<bool>(
                name: "is_approved",
                table: "transfers",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);
        }
    }
}
