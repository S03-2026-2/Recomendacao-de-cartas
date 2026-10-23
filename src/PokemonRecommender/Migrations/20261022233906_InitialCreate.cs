using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace PokemonRecommender.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "Cartas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tipos = table.Column<List<string>>(type: "text[]", nullable: false),
                    Movimentos = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Regras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Texto = table.Column<string>(type: "text", nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(1536)", nullable: false),
                    Fonte = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Regras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoUsuario = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Nickname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DicaBloqueada = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RateLimits",
                columns: table => new
                {
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsoAtual = table.Column<int>(type: "integer", nullable: false),
                    InicioJanela = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Policy_LimiteAtual = table.Column<int>(type: "integer", nullable: false),
                    Policy_JanelaTempo = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateLimits", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_RateLimits_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Regras_Embedding",
                table: "Regras",
                column: "Embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Nickname",
                table: "Usuarios",
                column: "Nickname",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cartas");

            migrationBuilder.DropTable(
                name: "RateLimits");

            migrationBuilder.DropTable(
                name: "Regras");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
