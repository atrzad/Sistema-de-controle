# Dados de exemplo

`agenda-professores.sql` é um dump (`pg_dump --data-only --column-inserts`) com uma agenda
real de exemplo já carregada no sistema: **35 professores**, **24 turmas**, **24 salas** (uma
por turma) e **663 aulas agendadas** no cronograma semanal, extraída de uma grade horária
escolar real. Também inclui alguns registros de frequência (`registros_frequencia`) usados
para testar a tela de Turmas Afetadas.

Este dump existe para permitir recriar o mesmo estado de dados em qualquer ambiente (dev,
demonstração, outro banco), sem depender de um banco específico já populado.

## Como restaurar

Com o banco já criado e as migrations aplicadas (`docker compose up -d` cuida disso
automaticamente no startup da API):

```bash
docker compose exec -T db psql -U sistema_controle_user -d sistema_controle < seed-data/agenda-professores.sql
```

> Os `INSERT` assumem os IDs originais (sequências ajustadas ao final do dump). Rode isso
> logo após subir um banco **vazio** — se já houver professores/turmas/salas cadastrados,
> pode haver conflito de chave única (email, matrícula, nome de sala, etc.).

## Como os dados foram gerados

A agenda foi extraída de PDFs escolares (nome do professor, matéria, dia da semana e
horário de início por turma) e importada via o script [`../scripts/import-agenda.mjs`](../scripts/import-agenda.mjs),
que:

1. Faz login na API e cria os 35 professores.
2. Cria uma turma e uma sala para cada código de turma encontrado na agenda (ex: `9º4`) —
   a fonte original não trazia informação de sala, então foi usada uma sala dedicada por
   turma como aproximação razoável.
3. Cria as aulas agendadas (`POST /cronograma`), convertendo os horários de início listados
   em uma grade fixa de períodos (`07:00–08:20`, `08:20–09:20`, `09:20–10:20`, `10:20–11:10`,
   `13:00–14:00`, `14:00–15:00`, `15:00–15:50`).

Do total de aulas processadas, **70 foram rejeitadas pelo próprio sistema com HTTP 409**
por conflito de horário — casos em que a mesma turma aparecia com dois professores/matérias
diferentes no mesmo dia e horário na fonte original (provável inconsistência da extração
automática dos PDFs). Isso é esperado e demonstra a checagem de conflito do
`CronogramaService` funcionando corretamente.
