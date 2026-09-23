using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaDeControle.Infrastructure.Data.Migrations
{
    /// <summary>
    /// Leva para o banco as regras de integridade que antes só existiam no código:
    /// - exclusion constraints impedem aulas ativas sobrepostas na mesma sala ou para o mesmo
    ///   professor no mesmo dia (mesma regra de CronogramaService.ValidarConflitosAsync, mas
    ///   segura contra requisições simultâneas);
    /// - o token de concorrência xmin é coluna de sistema do Postgres, então não gera DDL.
    /// </summary>
    public partial class ConstraintsDeIntegridade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            // Intervalo [HoraInicio, HoraFim): aulas encostadas (08:00-09:00 e 09:00-10:00) não conflitam.
            migrationBuilder.Sql("""
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'intervalo_horario') THEN
                        CREATE TYPE intervalo_horario AS RANGE (subtype = time);
                    END IF;
                END
                $$;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE aulas_agendadas
                    ADD CONSTRAINT ex_aulas_agendadas_sala_horario
                    EXCLUDE USING gist (
                        "SalaId" WITH =,
                        "DiaSemana" WITH =,
                        intervalo_horario("HoraInicio", "HoraFim") WITH &&
                    ) WHERE ("Ativo");
                """);

            migrationBuilder.Sql("""
                ALTER TABLE aulas_agendadas
                    ADD CONSTRAINT ex_aulas_agendadas_professor_horario
                    EXCLUDE USING gist (
                        "ProfessorId" WITH =,
                        "DiaSemana" WITH =,
                        intervalo_horario("HoraInicio", "HoraFim") WITH &&
                    ) WHERE ("Ativo");
                """);

            migrationBuilder.Sql("""
                ALTER TABLE aulas_agendadas
                    ADD CONSTRAINT ck_aulas_agendadas_horario CHECK ("HoraFim" > "HoraInicio");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE aulas_agendadas DROP CONSTRAINT IF EXISTS ck_aulas_agendadas_horario;");
            migrationBuilder.Sql("ALTER TABLE aulas_agendadas DROP CONSTRAINT IF EXISTS ex_aulas_agendadas_professor_horario;");
            migrationBuilder.Sql("ALTER TABLE aulas_agendadas DROP CONSTRAINT IF EXISTS ex_aulas_agendadas_sala_horario;");
            migrationBuilder.Sql("DROP TYPE IF EXISTS intervalo_horario;");
        }
    }
}
