using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SKILLS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SKILLS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    CARGO_ATUAL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CARGO_DESEJADO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CURSOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Provedor = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    CargaHoraria = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SkillId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CURSOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CURSOS_SKILLS_SkillId",
                        column: x => x.SkillId,
                        principalTable: "SKILLS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS_CURSOS",
                columns: table => new
                {
                    CursosAtivosId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    UsuariosMatriculadosId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS_CURSOS", x => new { x.CursosAtivosId, x.UsuariosMatriculadosId });
                    table.ForeignKey(
                        name: "FK_USUARIOS_CURSOS_CURSOS_CursosAtivosId",
                        column: x => x.CursosAtivosId,
                        principalTable: "CURSOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USUARIOS_CURSOS_USUARIOS_UsuariosMatriculadosId",
                        column: x => x.UsuariosMatriculadosId,
                        principalTable: "USUARIOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CURSOS_SkillId",
                table: "CURSOS",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_CURSOS_UsuariosMatriculadosId",
                table: "USUARIOS_CURSOS",
                column: "UsuariosMatriculadosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "USUARIOS_CURSOS");

            migrationBuilder.DropTable(
                name: "CURSOS");

            migrationBuilder.DropTable(
                name: "USUARIOS");

            migrationBuilder.DropTable(
                name: "SKILLS");
        }
    }
}
