using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clima.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tb_Registos",
                columns: table => new
                {
                    ID_registo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    temperatura = table.Column<int>(type: "int", nullable: false),
                    humidade = table.Column<int>(type: "int", nullable: false),
                    risco_temperatura = table.Column<int>(type: "int", nullable: false),
                    risco_humidade = table.Column<int>(type: "int", nullable: false),
                    data_registo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tb_Registos", x => x.ID_registo);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tb_Registos");
        }
    }
}
