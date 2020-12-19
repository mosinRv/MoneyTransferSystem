using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MoneyTransferSystem.Migrations
{
    public partial class AccNowHasSingleCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_UserLogin",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalMoneyRules_Currencies_CurrencyName",
                table: "GlobalMoneyRules");

            migrationBuilder.DropForeignKey(
                name: "FK_MoneyRules_Currencies_CurrencyName",
                table: "MoneyRules");

            migrationBuilder.DropForeignKey(
                name: "FK_MoneyRules_Users_UserLogin",
                table: "MoneyRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_MoneyRules_CurrencyName",
                table: "MoneyRules");

            migrationBuilder.DropIndex(
                name: "IX_MoneyRules_UserLogin",
                table: "MoneyRules");

            migrationBuilder.DropIndex(
                name: "IX_GlobalMoneyRules_CurrencyName",
                table: "GlobalMoneyRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserLogin",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "MoneyRules");

            migrationBuilder.DropColumn(
                name: "UserLogin",
                table: "MoneyRules");

            migrationBuilder.DropColumn(
                name: "CurrencyName",
                table: "GlobalMoneyRules");

            migrationBuilder.DropColumn(
                name: "DollarEquivalent",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "UserLogin",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "MoneyRules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MoneyRules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Comission",
                table: "GlobalMoneyRules",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "GlobalMoneyRules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Currencies",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "CharCode",
                table: "Currencies",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Money",
                table: "Accounts",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Accounts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CharCode", "Name" },
                values: new object[] { 1, "USD", "USA dollar" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "Pass", "isAdmin" },
                values: new object[] { 1, "Adam", "123", false });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MoneyRules_CurrencyId",
                table: "MoneyRules",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyRules_UserId",
                table: "MoneyRules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalMoneyRules_CurrencyId",
                table: "GlobalMoneyRules",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_CharCode",
                table: "Currencies",
                column: "CharCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Currencies_CurrencyId",
                table: "Accounts",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_UserId",
                table: "Accounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalMoneyRules_Currencies_CurrencyId",
                table: "GlobalMoneyRules",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoneyRules_Currencies_CurrencyId",
                table: "MoneyRules",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoneyRules_Users_UserId",
                table: "MoneyRules",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Currencies_CurrencyId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Users_UserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_GlobalMoneyRules_Currencies_CurrencyId",
                table: "GlobalMoneyRules");

            migrationBuilder.DropForeignKey(
                name: "FK_MoneyRules_Currencies_CurrencyId",
                table: "MoneyRules");

            migrationBuilder.DropForeignKey(
                name: "FK_MoneyRules_Users_UserId",
                table: "MoneyRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Login",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_MoneyRules_CurrencyId",
                table: "MoneyRules");

            migrationBuilder.DropIndex(
                name: "IX_MoneyRules_UserId",
                table: "MoneyRules");

            migrationBuilder.DropIndex(
                name: "IX_GlobalMoneyRules_CurrencyId",
                table: "GlobalMoneyRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_CharCode",
                table: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyColumnType: "integer",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyColumnType: "integer",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "MoneyRules");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MoneyRules");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GlobalMoneyRules");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CharCode",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Login",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "MoneyRules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLogin",
                table: "MoneyRules",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Comission",
                table: "GlobalMoneyRules",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyName",
                table: "GlobalMoneyRules",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DollarEquivalent",
                table: "Currencies",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "Money",
                table: "Accounts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "UserLogin",
                table: "Accounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Login");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyRules_CurrencyName",
                table: "MoneyRules",
                column: "CurrencyName");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyRules_UserLogin",
                table: "MoneyRules",
                column: "UserLogin");

            migrationBuilder.CreateIndex(
                name: "IX_GlobalMoneyRules_CurrencyName",
                table: "GlobalMoneyRules",
                column: "CurrencyName");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserLogin",
                table: "Accounts",
                column: "UserLogin");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Users_UserLogin",
                table: "Accounts",
                column: "UserLogin",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GlobalMoneyRules_Currencies_CurrencyName",
                table: "GlobalMoneyRules",
                column: "CurrencyName",
                principalTable: "Currencies",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MoneyRules_Currencies_CurrencyName",
                table: "MoneyRules",
                column: "CurrencyName",
                principalTable: "Currencies",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MoneyRules_Users_UserLogin",
                table: "MoneyRules",
                column: "UserLogin",
                principalTable: "Users",
                principalColumn: "Login",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
