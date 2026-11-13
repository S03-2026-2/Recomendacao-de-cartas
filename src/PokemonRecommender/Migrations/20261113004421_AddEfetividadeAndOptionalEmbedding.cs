using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace PokemonRecommender.Migrations
{
    /// <inheritdoc />
    public partial class AddEfetividadeAndOptionalEmbedding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "Regras",
                type: "vector(1536)",
                nullable: true,
                oldClrType: typeof(Vector),
                oldType: "vector(1536)");

            migrationBuilder.CreateTable(
                name: "Efetividades",
                columns: table => new
                {
                    Atacante = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Defensor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Multiplicador = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Efetividades", x => new { x.Atacante, x.Defensor });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regras_Fonte",
                table: "Regras",
                column: "Fonte");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Efetividades");

            migrationBuilder.DropIndex(
                name: "IX_Regras_Fonte",
                table: "Regras");

            migrationBuilder.AlterColumn<Vector>(
                name: "Embedding",
                table: "Regras",
                type: "vector(1536)",
                nullable: false,
                oldClrType: typeof(Vector),
                oldType: "vector(1536)",
                oldNullable: true);
        }
    }
}
