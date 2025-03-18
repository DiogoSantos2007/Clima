using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clima.Migrations
{
    /// <inheritdoc />
    public partial class init_v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "radiacao",
                table: "Tb_Registos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "risco_incendio",
                table: "Tb_Registos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "radiacao",
                table: "Tb_Registos");

            migrationBuilder.DropColumn(
                name: "risco_incendio",
                table: "Tb_Registos");
        }
    }
}
