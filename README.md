# Sistema de Controle

Sistema para controle de frequência dos professores e das turmas afetadas pela sua
ausência. MVP funcional: backend, frontend e infraestrutura Docker implementados
(ver [`docs/ARQUITETURA.md`](docs/ARQUITETURA.md) para o desenho completo).

## Funcionalidades

- Cadastro de quadro de professores
- Cadastro de turmas e salas
- Montagem de cronograma (grade horária semanal) para professores vinculados a turmas/salas,
  com checagem automática de conflito de horário (sala e professor)
- Visualização do cronograma por três visões: geral, por professor e por sala
- Filtro de professores por turma e filtro de turmas por professor
- Registro diário de frequência (presença/ausência) por aula agendada
- Rastreador de frequência semanal, mensal e consolidado
- Mapeamento de turmas afetadas pela ausência de professores, com resumo semanal
  (quantas turmas distintas e quantas ocorrências de ausência na semana)
- Justificativa de ausência, com upload/download de anexo de atestado
- Filtros da interface (cronograma, frequência, turmas afetadas) persistem no navegador
  entre recarregamentos de página

## Stack Tecnológica

| Camada | Tecnologia Escolhida | Papel no Sistema |
|---|---|---|
| Frontend (Interface) | React.js + TypeScript | Telas de cadastro, dashboards com filtros cruzados (professores/turmas) e exibição de cronogramas |
| Backend (API) | ASP.NET Core (C#) | Regras de negócio, endpoints e lógica de rastreamento de frequência e ausências |
| Banco de Dados | PostgreSQL | Armazenamento relacional dos dados estruturados (professores, salas, turmas e registros de frequência) |
| Mapeamento de Dados (ORM) | Entity Framework Core | Comunicação entre o C# e o PostgreSQL, consultas e controle de migrações do banco |
| Validação de Dados | FluentValidation | Validação limpa e organizada dos dados enviados nos cadastros (professores, turmas, etc.) |
| Infraestrutura / DevOps | Docker & Docker Compose | Isolar e subir o banco de dados e a API de forma integrada com um único comando |

## Arquitetura

O sistema é um monorepo com `backend/` (ASP.NET Core em camadas Api/Application/Domain/
Infrastructure) e `frontend/` (React + TypeScript + Vite), orquestrados via Docker
Compose. O desenho completo — estrutura de diretórios, modelo de domínio, endpoints da
API e roadmap de implementação — está em [`docs/ARQUITETURA.md`](docs/ARQUITETURA.md).

## Como Rodar

### Opção 1 — Docker Compose (stack completa: banco + API + frontend)

```bash
cp .env.example .env
# edite o .env: DB_PASSWORD, JWT_SECRET (openssl rand -base64 48), ADMIN_EMAIL, ADMIN_PASSWORD
docker compose up -d --build
```

- Frontend: http://localhost:3000 (a API responde no mesmo endereço, em `/api/v1`, via proxy do nginx)
- Login inicial: `ADMIN_EMAIL` / `ADMIN_PASSWORD` do `.env` — troque a senha em **Alterar senha** (canto superior direito)
- O compose recusa subir se `DB_PASSWORD`, `JWT_SECRET`, `ADMIN_EMAIL` ou `ADMIN_PASSWORD` não estiverem definidos

Para expor também o banco (porta 5433) e a API (porta 5000) no host e ligar o Swagger
(http://localhost:5000/swagger), use a sobreposição de desenvolvimento:

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml up -d --build
```

### Opção 2 — Desenvolvimento local (hot reload)

Suba apenas o banco via Docker e rode API/frontend localmente:

```bash
docker compose -f docker-compose.yml -f docker-compose.dev.yml up -d db
```

**Backend** (requer .NET 8 SDK). `appsettings.Development.json` aponta para o banco em
`localhost:5433` com senha `changeme` — use `DB_PASSWORD=changeme` no `.env` ou ajuste a
connection string:

```bash
cd backend
dotnet ef database update --project src/SistemaDeControle.Infrastructure --startup-project src/SistemaDeControle.Api
dotnet watch --project src/SistemaDeControle.Api run
```

**Frontend** (requer Node.js 20+). O Vite faz proxy de `/api` para `http://localhost:5000`;
se a API estiver rodando com `dotnet run` (porta 5141), defina `VITE_API_PROXY_TARGET`:

```bash
cd frontend
cp .env.example .env.local
npm install
VITE_API_PROXY_TARGET=http://localhost:5141 npm run dev
```

Acesse em http://localhost:5173.

## Versão desktop (Windows .exe)

Pacote para rodar em um único computador, sem Docker nem instalação: um executável com a
API e o frontend, mais um PostgreSQL portátil que sobe e desce junto com o programa.

```bash
scripts/build-desktop.sh            # gera dist/SistemaDeControle-win-x64.zip (pode rodar no Linux/WSL)
scripts/build-desktop.sh linux-x64  # mesma coisa para Linux, útil para testar sem Windows
```

No Windows: extrair o zip, dar dois cliques em `SistemaDeControle.exe`, informar e-mail e
senha do administrador na primeira execução, e o navegador abre em http://localhost:5080.
Os dados ficam em `%LOCALAPPDATA%\SistemaDeControle`. Instruções completas para o usuário
final em [`scripts/desktop/LEIA-ME.txt`](scripts/desktop/LEIA-ME.txt), que vai dentro do zip.
O workflow `.github/workflows/desktop.yml` gera o zip no GitHub (manual ou por tag `v*`).

## Conta de administrador

A conta de `ADMIN_EMAIL` é **sempre garantida**: se não existir no banco, é criada no start
da API com a senha `ADMIN_PASSWORD`. Depois do primeiro login, a senha pode ser trocada em
**Alterar senha**.

Esqueceu a senha?
- **Servidor/Docker:** coloque `RESET_ADMIN_PASSWORD=true` no `.env` e rode `docker compose up -d`.
  A senha volta a ser `ADMIN_PASSWORD`. Entre e volte a opção para `false`.
- **Desktop:** dois cliques em `Redefinir senha do administrador.bat` (ou
  `SistemaDeControle.exe --redefinir-senha`) e digite a nova senha.

## Deploy em produção (VPS)

Pré-requisitos: servidor Linux com Docker + plugin Compose, um domínio com registro DNS
`A`/`AAAA` apontando para o IP do servidor, e portas 80/443 liberadas no firewall (as demais
podem ficar fechadas — banco e API não são publicados).

```bash
git clone <repo> && cd Sistema-de-controle
cp .env.example .env
# preencha DB_PASSWORD, JWT_SECRET, ADMIN_EMAIL, ADMIN_PASSWORD e DOMAIN
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build
docker compose ps        # todos os serviços devem ficar "healthy"/"running"
```

O Caddy obtém e renova o certificado HTTPS automaticamente. Depois do primeiro acesso,
entre com o usuário inicial e **troque a senha**.

Arquitetura em produção:

```
internet ──443──▶ Caddy (HTTPS) ──▶ nginx/frontend ──/api──▶ API (.NET) ──▶ Postgres
                                                              └──▶ volume de atestados
```

**Atualizar a versão:** `git pull && docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build`.
As migrations do banco são aplicadas automaticamente no start da API — faça um backup antes.

**Backup:** `scripts/backup.sh /var/backups/sistema-controle` gera o dump do banco e um
`.tar.gz` dos atestados (retém 14 dias). Agende no cron (exemplo no cabeçalho do script) e
copie os arquivos para fora do servidor. Para restaurar: `scripts/restore.sh <db.dump> <anexos.tar.gz>`.

> **Volumes criados antes desta versão:** a API agora roda como usuário não-root (UID 1654).
> Se o volume `atestados-storage` já existia, ajuste o dono uma vez:
> `docker compose run --rm --user root api chown -R 1654 /app/storage`

### Testes do backend

```bash
cd backend
dotnet test
```

Os testes de integração (`tests/SistemaDeControle.IntegrationTests`) sobem um Postgres
descartável via Testcontainers — é preciso ter o Docker rodando. Sem Docker, aponte para um
Postgres existente: `SDC_TEST_PG="Host=localhost;Port=5432;Username=postgres;Password=..." dotnet test`
(cada execução cria e apaga um banco temporário). O CI
(`.github/workflows/ci.yml`) roda build, testes e build das imagens a cada push/PR.

## Dados de exemplo

O projeto inclui uma agenda escolar real de exemplo (35 professores, 24 turmas/salas e
663 aulas no cronograma) para testar o sistema com dados realistas:

- [`seed-data/`](seed-data/) — dump SQL pronto para restaurar direto no Postgres
- [`scripts/import-agenda.mjs`](scripts/import-agenda.mjs) — script Node que recria os
  mesmos dados chamando a API (login → professores → turmas/salas → cronograma)

Instruções de uso em [`seed-data/README.md`](seed-data/README.md).
