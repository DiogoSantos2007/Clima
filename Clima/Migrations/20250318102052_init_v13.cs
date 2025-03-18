using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clima.Migrations
{
    /// <inheritdoc />
    public partial class init_v13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "humidade_solo",
                table: "Tb_Registos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "risco_humidade_solo",
                table: "Tb_Registos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "humidade_solo",
                table: "Tb_Registos");

            migrationBuilder.DropColumn(
                name: "risco_humidade_solo",
                table: "Tb_Registos");
        }
    }
}
