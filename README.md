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
docker compose up -d --build
```

- Frontend: http://localhost:3000
- API: http://localhost:5000/api/v1 (Swagger em http://localhost:5000/swagger, ambiente Development)
- Usuário pedagogo inicial (criado automaticamente no primeiro start): `pedagogo@sistemadecontrole.local` / `TrocarSenha123!`

### Opção 2 — Desenvolvimento local (hot reload)

Suba apenas o banco via Docker e rode API/frontend localmente:

```bash
docker compose up -d db
```

**Backend** (requer .NET 8 SDK):

```bash
cd backend
dotnet ef database update --project src/SistemaDeControle.Infrastructure --startup-project src/SistemaDeControle.Api
dotnet watch --project src/SistemaDeControle.Api run
```

**Frontend** (requer Node.js 20+):

```bash
cd frontend
cp .env.example .env.local
npm install
npm run dev
```

Acesse em http://localhost:5173.

### Testes do backend

```bash
cd backend
dotnet test
```

## Dados de exemplo

O projeto inclui uma agenda escolar real de exemplo (35 professores, 24 turmas/salas e
663 aulas no cronograma) para testar o sistema com dados realistas:

- [`seed-data/`](seed-data/) — dump SQL pronto para restaurar direto no Postgres
- [`scripts/import-agenda.mjs`](scripts/import-agenda.mjs) — script Node que recria os
  mesmos dados chamando a API (login → professores → turmas/salas → cronograma)

Instruções de uso em [`seed-data/README.md`](seed-data/README.md).
