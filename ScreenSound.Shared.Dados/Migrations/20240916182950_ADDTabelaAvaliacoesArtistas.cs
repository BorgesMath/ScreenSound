using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScreenSound.Migrations
{
    /// <inheritdoc />
    public partial class ADDTabelaAvaliacoesArtistas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvaliacoesArtistas",
                columns: table => new
                {
                    ArtistaID = table.Column<int>(type: "int", nullable: false),
                    PessoaID = table.Column<int>(type: "int", nullable: false),
                    Nota = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesArtistas", x => new { x.ArtistaID, x.PessoaID });
                    table.ForeignKey(
                        name: "FK_AvaliacoesArtistas_Artistas_ArtistaID",
                        column: x => x.ArtistaID,
                        principalTable: "Artistas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvaliacoesArtistas");
        }
    }
}
