using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capsule.Data.Migrations
{
    public partial class ProprietaireDeVetement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProprietaireId",
                table: "Vetement",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProprietaireId",
                table: "Vetement");
        }
    }
}
