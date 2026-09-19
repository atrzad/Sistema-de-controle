using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaDeControle.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "professores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Matricula = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Disciplina = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Capacidade = table.Column<int>(type: "integer", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "turmas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Turno = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AnoLetivo = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turmas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "aulas_agendadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProfessorId = table.Column<int>(type: "integer", nullable: false),
                    TurmaId = table.Column<int>(type: "integer", nullable: false),
                    SalaId = table.Column<int>(type: "integer", nullable: false),
                    DiaSemana = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HoraFim = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Disciplina = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    VigenteDesde = table.Column<DateOnly>(type: "date", nullable: true),
                    VigenteAte = table.Column<DateOnly>(type: "date", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aulas_agendadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aulas_agendadas_professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_aulas_agendadas_salas_SalaId",
                        column: x => x.SalaId,
                        principalTable: "salas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_aulas_agendadas_turmas_TurmaId",
                        column: x => x.TurmaId,
                        principalTable: "turmas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "registros_frequencia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AulaAgendadaId = table.Column<int>(type: "integer", nullable: false),
                    ProfessorId = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RegistradoPorUsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registros_frequencia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_registros_frequencia_aulas_agendadas_AulaAgendadaId",
                        column: x => x.AulaAgendadaId,
                        principalTable: "aulas_agendadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_frequencia_professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_registros_frequencia_usuarios_RegistradoPorUsuarioId",
                        column: x => x.RegistradoPorUsuarioId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "justificativas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistroFrequenciaId = table.Column<int>(type: "integer", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_justificativas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_justificativas_registros_frequencia_RegistroFrequenciaId",
                        column: x => x.RegistroFrequenciaId,
                        principalTable: "registros_frequencia",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "anexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JustificativaId = table.Column<int>(type: "integer", nullable: false),
                    NomeArquivoOriginal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NomeArquivoArmazenado = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CaminhoRelativo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoConteudo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_anexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_anexos_justificativas_JustificativaId",
                        column: x => x.JustificativaId,
                        principalTable: "justificativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_anexos_JustificativaId",
                table: "anexos",
                column: "JustificativaId");

            migrationBuilder.CreateIndex(
                name: "IX_aulas_agendadas_ProfessorId_DiaSemana",
                table: "aulas_agendadas",
                columns: new[] { "ProfessorId", "DiaSemana" });

            migrationBuilder.CreateIndex(
                name: "IX_aulas_agendadas_SalaId_DiaSemana",
                table: "aulas_agendadas",
                columns: new[] { "SalaId", "DiaSemana" });

            migrationBuilder.CreateIndex(
                name: "IX_aulas_agendadas_TurmaId_DiaSemana",
                table: "aulas_agendadas",
                columns: new[] { "TurmaId", "DiaSemana" });

            migrationBuilder.CreateIndex(
                name: "IX_justificativas_RegistroFrequenciaId",
                table: "justificativas",
                column: "RegistroFrequenciaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professores_Email",
                table: "professores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professores_Matricula",
                table: "professores",
                column: "Matricula",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_registros_frequencia_AulaAgendadaId_Data",
                table: "registros_frequencia",
                columns: new[] { "AulaAgendadaId", "Data" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_registros_frequencia_Data",
                table: "registros_frequencia",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_registros_frequencia_ProfessorId",
                table: "registros_frequencia",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_registros_frequencia_RegistradoPorUsuarioId",
                table: "registros_frequencia",
                column: "RegistradoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_salas_Nome",
                table: "salas",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_turmas_Nome_AnoLetivo_Turno",
                table: "turmas",
                columns: new[] { "Nome", "AnoLetivo", "Turno" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_Email",
                table: "usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "anexos");

            migrationBuilder.DropTable(
                name: "justificativas");

            migrationBuilder.DropTable(
                name: "registros_frequencia");

            migrationBuilder.DropTable(
                name: "aulas_agendadas");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "professores");

            migrationBuilder.DropTable(
                name: "salas");

            migrationBuilder.DropTable(
                name: "turmas");
        }
    }
}
