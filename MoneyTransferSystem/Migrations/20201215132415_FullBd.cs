using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MoneyTransferSystem.Migrations
{
    public partial class FullBd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Name = table.Column<string>(type: "text", nullable: false),
                    DollarEquivalent = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "GlobalMoneyRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Min = table.Column<double>(type: "double precision", nullable: false),
                    Max = table.Column<double>(type: "double precision", nullable: false),
                    Comission = table.Column<double>(type: "double precision", nullable: false),
                    isComissionFixed = table.Column<bool>(type: "boolean", nullable: false),
                    CurrencyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalMoneyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlobalMoneyRules_Currencies_CurrencyName",
                        column: x => x.CurrencyName,
                        principalTable: "Currencies",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MoneyRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Min = table.Column<double>(type: "double precision", nullable: false),
                    Max = table.Column<double>(type: "double precision", nullable: false),
                    Comission = table.Column<double>(type: "double precision", nullable: false),
                    isComissionFixed = table.Column<bool>(type: "boolean", nullable: false),
                    UserLogin = table.Column<string>(type: "text", nullable: true),
                    CurrencyName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoneyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoneyRules_Currencies_CurrencyName",
                        column: x => x.CurrencyName,
                        principalTable: "Currencies",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MoneyRules_Users_UserLogin",
                        column: x => x.UserLogin,
                        principalTable: "Users",
                        principalColumn: "Login",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GlobalMoneyRules_CurrencyName",
                table: "GlobalMoneyRules",
                column: "CurrencyName");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyRules_CurrencyName",
                table: "MoneyRules",
                column: "CurrencyName");

            migrationBuilder.CreateIndex(
                name: "IX_MoneyRules_UserLogin",
                table: "MoneyRules",
                column: "UserLogin");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalMoneyRules");

            migrationBuilder.DropTable(
                name: "MoneyRules");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
