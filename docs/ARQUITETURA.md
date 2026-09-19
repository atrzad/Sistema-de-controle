# Arquitetura — Sistema de Controle

> **Status**: implementado. Este documento descreve tanto o desenho original quanto o
> estado atual do sistema (backend .NET 8, frontend React, Docker Compose funcionando
> ponta a ponta). Refinamentos feitos durante a implementação, além do desenho original,
> estão marcados com **[implementado]** ao longo do texto.

## 0. Visão Geral e Premissas

- **Domínio**: controle de frequência de professores e mapeamento de turmas afetadas por ausências.
- **Usuários**: apenas pedagogos (perfil único), autenticados via JWT, sem RBAC.
- **Stack**: React + TypeScript (Vite) no frontend; ASP.NET Core Web API (C#) no backend; PostgreSQL + EF Core (migrations); FluentValidation; Docker + Docker Compose.
- **Abordagem arquitetural**: camadas pragmáticas (Controllers → Services → Repositories/DbContext) dentro de uma solução .NET modesta, evitando Clean Architecture "completa" (sem CQRS/MediatR/Domain Events), já que o domínio é de porte pequeno/médio.
- **Monorepo**: um único repositório Git contendo `backend/` e `frontend/`, orquestrados por `docker-compose.yml` na raiz.

---

## 1. Estrutura de Diretórios do Monorepo

```
Sistema-de-controle/
├── README.md
├── docker-compose.yml
├── docker-compose.override.yml        # overrides de dev (hot reload, volumes)
├── .env.example
├── .gitignore
│
├── backend/
│   ├── SistemaDeControle.sln
│   ├── Dockerfile
│   ├── .dockerignore
│   │
│   ├── src/
│   │   ├── SistemaDeControle.Api/                     # Projeto de entrada (Web API)
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── Controllers/
│   │   │   │   ├── AuthController.cs
│   │   │   │   ├── ProfessoresController.cs
│   │   │   │   ├── TurmasController.cs
│   │   │   │   ├── SalasController.cs
│   │   │   │   ├── CronogramaController.cs
│   │   │   │   ├── FrequenciaController.cs
│   │   │   │   ├── JustificativasController.cs
│   │   │   │   └── RelatoriosController.cs
│   │   │   ├── Middlewares/
│   │   │   │   └── ExceptionHandlingMiddleware.cs
│   │   │   ├── Extensions/
│   │   │   │   ├── ServiceCollectionExtensions.cs     # DI setup (services, repos, validators)
│   │   │   │   └── AuthenticationExtensions.cs        # JWT config
│   │   │   └── Filters/
│   │   │       └── ValidationFilter.cs
│   │   │
│   │   ├── SistemaDeControle.Application/             # Regras de negócio / serviços / DTOs
│   │   │   ├── DTOs/
│   │   │   │   ├── Professores/
│   │   │   │   ├── Turmas/
│   │   │   │   ├── Salas/
│   │   │   │   ├── Cronograma/
│   │   │   │   ├── Frequencia/
│   │   │   │   ├── Justificativas/
│   │   │   │   └── Relatorios/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IProfessorService.cs
│   │   │   │   ├── ITurmaService.cs
│   │   │   │   ├── ISalaService.cs
│   │   │   │   ├── ICronogramaService.cs
│   │   │   │   ├── IFrequenciaService.cs
│   │   │   │   ├── IJustificativaService.cs
│   │   │   │   ├── IRelatorioService.cs
│   │   │   │   └── IArquivoStorageService.cs
│   │   │   ├── Services/
│   │   │   │   ├── ProfessorService.cs
│   │   │   │   ├── TurmaService.cs
│   │   │   │   ├── SalaService.cs
│   │   │   │   ├── CronogramaService.cs
│   │   │   │   ├── FrequenciaService.cs              # lógica de ausência + turmas afetadas
│   │   │   │   ├── JustificativaService.cs
│   │   │   │   ├── RelatorioService.cs                # relatórios semanal/mensal
│   │   │   │   └── AuthService.cs
│   │   │   ├── Validators/                            # FluentValidation
│   │   │   │   ├── ProfessorCreateDtoValidator.cs
│   │   │   │   ├── TurmaCreateDtoValidator.cs
│   │   │   │   ├── CronogramaCreateDtoValidator.cs
│   │   │   │   ├── FrequenciaCreateDtoValidator.cs
│   │   │   │   └── JustificativaCreateDtoValidator.cs
│   │   │   └── Common/
│   │   │       ├── Result.cs / OperationResult.cs     # padrão de retorno (sucesso/erro)
│   │   │       └── Exceptions/                        # NotFoundException, ConflictException, ...
│   │   │
│   │   ├── SistemaDeControle.Domain/                  # Entidades e enums puros
│   │   │   ├── Entities/
│   │   │   │   ├── Professor.cs
│   │   │   │   ├── Turma.cs
│   │   │   │   ├── Sala.cs
│   │   │   │   ├── AulaAgendada.cs                    # item do cronograma
│   │   │   │   ├── RegistroFrequencia.cs              # ausência/presença
│   │   │   │   ├── Justificativa.cs
│   │   │   │   ├── Anexo.cs
│   │   │   │   └── Usuario.cs                         # pedagogo (login)
│   │   │   ├── Enums/
│   │   │   │   ├── DiaSemana.cs
│   │   │   │   ├── StatusFrequencia.cs                # Presente, Ausente, AusenciaJustificada
│   │   │   │   └── StatusJustificativa.cs             # opcional
│   │   │   └── Common/
│   │   │       └── BaseEntity.cs                      # Id, CreatedAt, UpdatedAt
│   │   │
│   │   └── SistemaDeControle.Infrastructure/          # EF Core, repositórios, storage
│   │       ├── Data/
│   │       │   ├── AppDbContext.cs
│   │       │   ├── Configurations/                    # IEntityTypeConfiguration<T> por entidade
│   │       │   │   ├── ProfessorConfiguration.cs
│   │       │   │   ├── TurmaConfiguration.cs
│   │       │   │   ├── SalaConfiguration.cs
│   │       │   │   ├── AulaAgendadaConfiguration.cs
│   │       │   │   ├── RegistroFrequenciaConfiguration.cs
│   │       │   │   ├── JustificativaConfiguration.cs
│   │       │   │   └── UsuarioConfiguration.cs
│   │       │   ├── Migrations/                        # geradas pelo EF Core
│   │       │   └── Seed/
│   │       │       └── DbSeeder.cs                    # usuário pedagogo inicial
│   │       ├── Repositories/
│   │       │   ├── IRepository.cs                     # genérico, opcional
│   │       │   ├── ProfessorRepository.cs
│   │       │   ├── TurmaRepository.cs
│   │       │   ├── SalaRepository.cs
│   │       │   ├── CronogramaRepository.cs
│   │       │   ├── FrequenciaRepository.cs
│   │       │   ├── JustificativaRepository.cs
│   │       │   └── UsuarioRepository.cs
│   │       └── Storage/
│   │           └── LocalFileStorageService.cs         # implementa IArquivoStorageService (disco/volume)
│   │
│   └── tests/
│       ├── SistemaDeControle.UnitTests/
│       │   ├── Services/                              # CronogramaServiceTests, FrequenciaServiceTests, ...
│       │   └── Validators/
│       └── SistemaDeControle.IntegrationTests/
│           └── Controllers/                           # WebApplicationFactory + Testcontainers Postgres
│
├── frontend/
│   ├── Dockerfile
│   ├── .dockerignore
│   ├── index.html
│   ├── vite.config.ts
│   ├── tsconfig.json
│   ├── package.json
│   ├── .env.example                                   # VITE_API_URL
│   └── src/
│       ├── main.tsx
│       ├── App.tsx
│       ├── routes/
│       │   ├── AppRoutes.tsx
│       │   └── PrivateRoute.tsx                       # guarda rota via JWT
│       ├── pages/
│       │   ├── login/
│       │   ├── professores/
│       │   ├── turmas/
│       │   ├── salas/
│       │   ├── cronograma/
│       │   ├── frequencia/
│       │   ├── ausencias/
│       │   └── dashboard/
│       ├── components/
│       │   ├── layout/                                # Sidebar, Header, MainLayout
│       │   ├── forms/                                 # inputs reutilizáveis
│       │   ├── filtros/                                # FiltroProfessorTurma
│       │   └── ui/                                     # Button, Table, Modal, FileUpload, etc.
│       ├── services/
│       │   ├── api.ts                                 # instância axios/fetch + interceptors JWT
│       │   ├── authService.ts
│       │   ├── professorService.ts
│       │   ├── turmaService.ts
│       │   ├── salaService.ts
│       │   ├── cronogramaService.ts
│       │   ├── frequenciaService.ts
│       │   ├── justificativaService.ts
│       │   └── relatorioService.ts
│       ├── types/
│       │   ├── professor.ts
│       │   ├── turma.ts
│       │   ├── sala.ts
│       │   ├── cronograma.ts
│       │   ├── frequencia.ts
│       │   └── justificativa.ts
│       ├── hooks/
│       │   ├── useAuth.ts
│       │   └── useFiltro.ts
│       ├── context/
│       │   └── AuthContext.tsx
│       └── utils/
│           ├── date.ts
│           └── validators.ts
│
└── docs/
    └── ARQUITETURA.md
```

**Justificativa da divisão em camadas .NET**: `Api` (apresentação/HTTP), `Application` (regras de negócio + DTOs + validação), `Domain` (entidades puras, sem dependência de EF), `Infrastructure` (EF Core, repositórios, storage de arquivos). É o meio-termo pragmático: separação de responsabilidades e testabilidade sem multiplicar abstrações desnecessárias.

---

## 2. Modelo de Domínio / Entidades

### 2.1 `Usuario` (Pedagogo — login)
| Campo | Tipo | Observação |
|---|---|---|
| Id | Guid/int | PK |
| Nome | string | |
| Email | string | único, usado no login |
| SenhaHash | string | hash (BCrypt/PasswordHasher) |
| CreatedAt | DateTime | |

> Sem tabela de Roles — perfil único implícito.

### 2.2 `Professor`
| Campo | Tipo |
|---|---|
| Id | Guid/int |
| Nome | string |
| Email | string |
| Telefone | string? |
| Matricula | string |
| Disciplina/Area | string? |
| Ativo | bool |
| CreatedAt / UpdatedAt | DateTime |

Relacionamentos: `Professor` 1—N `AulaAgendada`; `Professor` 1—N `RegistroFrequencia`.

### 2.3 `Turma`
| Campo | Tipo |
|---|---|
| Id | Guid/int |
| Nome | string (ex: "9º Ano B") |
| Turno | enum/string (Manhã, Tarde, Noite) |
| AnoLetivo | int |
| Ativo | bool |

Relacionamentos: `Turma` 1—N `AulaAgendada`.

### 2.4 `Sala`
| Campo | Tipo |
|---|---|
| Id | Guid/int |
| Nome/Codigo | string (ex: "Sala 12", "Lab. Informática") |
| Capacidade | int? |
| Ativo | bool |

### 2.5 `AulaAgendada` (o "Cronograma" — grade horária recorrente)

Entidade-chave que modela a recorrência semanal. Representa um "slot" fixo que se repete toda semana, não uma data específica.

| Campo | Tipo | Observação |
|---|---|---|
| Id | Guid/int | PK |
| ProfessorId | FK → Professor | |
| TurmaId | FK → Turma | |
| SalaId | FK → Sala | |
| DiaSemana | enum (Segunda..Domingo) | |
| HoraInicio | TimeOnly | ex: 08:00 |
| HoraFim | TimeOnly | ex: 10:00 |
| Disciplina | string? | opcional |
| VigenteDesde | DateOnly? | opcional — versionar por período letivo |
| VigenteAte | DateOnly? | opcional |
| Ativo | bool | |

**Regras de negócio** (validadas em `CronogramaService` + índice no banco):
- Não pode haver dois registros ativos com **mesma Sala + mesmo DiaSemana + horários sobrepostos**.
- Não pode haver dois registros ativos com **mesmo Professor + mesmo DiaSemana + horários sobrepostos**.
- Índices sugeridos: `(DiaSemana, SalaId, HoraInicio, HoraFim)` e `(DiaSemana, ProfessorId, HoraInicio, HoraFim)`.

Isso resolve o N:N "professor leciona várias turmas / turma tem vários professores em horários diferentes" como uma **tabela associativa enriquecida** (Professor × Turma × Sala × DiaSemana × Horário) — o padrão correto para grades horárias.

### 2.6 `RegistroFrequencia` (presença/ausência em uma ocorrência concreta)

Representa o registro real, em uma **data específica**, do que aconteceu com uma `AulaAgendada`.

| Campo | Tipo | Observação |
|---|---|---|
| Id | Guid/int | PK |
| AulaAgendadaId | FK → AulaAgendada | slot do cronograma |
| ProfessorId | FK → Professor | desnormalizado (útil se professor for substituído) |
| Data | DateOnly | data concreta |
| Status | enum (Presente, Ausente, AusenciaJustificada) | |
| RegistradoPorUsuarioId | FK → Usuario | pedagogo que registrou |
| Observacao | string? | |
| CreatedAt | DateTime | |

Constraint: único por `(AulaAgendadaId, Data)`.

> É a partir daqui que se calcula: frequência semanal/mensal por professor; turmas afetadas em uma data (join `RegistroFrequencia` com `Status = Ausente/AusenciaJustificada` → `AulaAgendada` → `Turma`/`Sala`).

### 2.7 `Justificativa`
| Campo | Tipo |
|---|---|
| Id | Guid/int |
| RegistroFrequenciaId | FK → RegistroFrequencia |
| Motivo | string |
| DataEnvio | DateTime |
| Status | enum? (opcional — pode ser simplificado a apenas "registrada") |

### 2.8 `Anexo`
| Campo | Tipo |
|---|---|
| Id | Guid/int |
| JustificativaId | FK → Justificativa |
| NomeArquivoOriginal | string |
| NomeArquivoArmazenado | string (nome único, ex: guid + extensão) |
| CaminhoRelativo | string |
| TipoConteudo | string (MimeType) |
| TamanhoBytes | long |
| UploadedAt | DateTime |

### 2.9 Diagrama relacional resumido

```
Usuario ─────< RegistroFrequencia (registrado por)

Professor ──1:N── AulaAgendada ──N:1── Turma
                       │
                       N:1
                       │
                     Sala

AulaAgendada ──1:N── RegistroFrequencia ──1:N── Justificativa ──1:N── Anexo
```

---

## 3. Endpoints da API REST

Prefixo base: `/api/v1`. Todos (exceto `/auth/login`) exigem `Authorization: Bearer <token>`.

### 3.1 AuthController (`/api/v1/auth`)
| Verbo | Rota | Descrição |
|---|---|---|
| POST | `/auth/login` | Autentica pedagogo (email+senha), retorna JWT |
| POST | `/auth/refresh` | (opcional) renova token via refresh token |
| GET | `/auth/me` | Retorna dados do usuário autenticado |

### 3.2 ProfessoresController (`/api/v1/professores`)
| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/professores` | Lista professores (`?ativo=&nome=`) |
| GET | `/professores/{id}` | Detalhe do professor |
| POST | `/professores` | Cria professor |
| PUT | `/professores/{id}` | Atualiza professor |
| DELETE | `/professores/{id}` | Inativa/remove professor |
| GET | `/professores/{id}/turmas` | Filtro: turmas vinculadas a este professor (via cronograma) |

### 3.3 TurmasController (`/api/v1/turmas`)
| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/turmas` | Lista turmas (`?anoLetivo=&turno=&ativo=`) |
| GET | `/turmas/{id}` | Detalhe |
| POST | `/turmas` | Cria turma |
| PUT | `/turmas/{id}` | Atualiza |
| DELETE | `/turmas/{id}` | Inativa/remove |
| GET | `/turmas/{id}/professores` | Filtro: professores que lecionam nesta turma (via cronograma) |

### 3.4 SalasController (`/api/v1/salas`)
| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/salas` | Lista salas |
| GET | `/salas/{id}` | Detalhe |
| POST | `/salas` | Cria sala |
| PUT | `/salas/{id}` | Atualiza |
| DELETE | `/salas/{id}` | Inativa/remove |

### 3.5 CronogramaController (`/api/v1/cronograma`)
| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/cronograma` | Lista aulas agendadas (`?professorId=&turmaId=&salaId=&diaSemana=`) |
| GET | `/cronograma/{id}` | Detalhe de uma aula agendada |
| POST | `/cronograma` | Cria vínculo Professor+Turma+Sala+Dia+Horário (valida conflitos) |
| PUT | `/cronograma/{id}` | Atualiza horário/vínculo |
| DELETE | `/cronograma/{id}` | Remove/inativa item do cronograma |
| GET | `/cronograma/grade-semanal` | Grade completa agrupada por dia (visão de calendário semanal) |
| GET | `/cronograma/professor/{professorId}` | Cronograma consolidado de um professor |
| GET | `/cronograma/turma/{turmaId}` | Cronograma consolidado de uma turma |

### 3.6 FrequenciaController (`/api/v1/frequencia`)
| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/frequencia` | Lista registros (`?professorId=&dataInicio=&dataFim=&status=`) |
| GET | `/frequencia/{id}` | Detalhe de um registro |
| POST | `/frequencia` | Registra presença/ausência para uma `AulaAgendada` em uma data |
| PUT | `/frequencia/{id}` | Atualiza status de um registro existente |
| DELETE | `/frequencia/{id}` | Remove registro (uso administrativo) |
| GET | `/frequencia/dia/{data}` | Ocorrências agendadas para a data + status já registrado (tela de marcação diária) |
| GET | `/frequencia/turmas-afetadas` | Mapeamento `?data=&professorId=` → turmas/salas sem aula naquele slot |

### 3.7 JustificativasController (`/api/v1/justificativas`)
| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/justificativas` | Lista (`?professorId=&dataInicio=&dataFim=`) |
| GET | `/justificativas/{id}` | Detalhe (com metadados do anexo) |
| POST | `/justificativas` | Cria justificativa (multipart/form-data: motivo + arquivo) |
| PUT | `/justificativas/{id}` | Atualiza motivo/status |
| DELETE | `/justificativas/{id}` | Remove justificativa |
| GET | `/justificativas/{id}/anexo` | Download/stream do arquivo do atestado |
| POST | `/justificativas/{id}/anexo` | Upload adicional de anexo |

### 3.8 RelatoriosController (`/api/v1/relatorios`)
| Verbo | Rota | Descrição |
|---|---|---|
| GET | `/relatorios/frequencia/semanal` | `?professorId=&dataReferencia=` → resumo semanal |
| GET | `/relatorios/frequencia/mensal` | `?professorId=&mes=&ano=` → resumo mensal (faltas, % presença) |
| GET | `/relatorios/frequencia/consolidado` | Visão geral de todos os professores em um período (dashboard) |
| GET | `/relatorios/turmas-afetadas` | Histórico de turmas afetadas em um intervalo de datas |

---

## 4. Arquitetura Geral e Fluxo de Comunicação

### 4.1 Fluxo padrão de uma requisição

```
Cliente (React)
   → HTTP Request (JSON ou multipart)
   → [Middleware: Exception Handling]
   → [Middleware: JWT Authentication/Authorization]
   → Controller (Api layer)
        - Recebe DTO
        - FluentValidation roda no pipeline (AddFluentValidationAutoValidation)
        - Chama Service correspondente
   → Service (Application layer)
        - Regra de negócio (ex: conflito de horário, turmas afetadas)
        - Orquestra 1+ Repository
        - Mapeia Entity → DTO de resposta
   → Repository (Infrastructure layer)
        - Encapsula acesso ao AppDbContext (EF Core)
   → PostgreSQL
   ← Retorno percorre o caminho inverso
   ← Controller retorna ActionResult padronizado (200/201/400/404/409/500)
```

### 4.2 FluentValidation

- Validadores registrados via `AddValidatorsFromAssemblyContaining<...>()`.
- Pipeline automático nos Controllers → 400 com `ValidationProblemDetails` antes de chegar ao Service.
- Regras cross-entity (ex: "já existe cronograma conflitante") ficam no **Service**, não no Validator — retornam 409.

### 4.3 Upload de atestado

- MVP: armazenamento em **volume Docker local**, via `IArquivoStorageService` → `LocalFileStorageService`.
- `POST /justificativas` aceita `multipart/form-data` (`registroFrequenciaId`, `motivo` + arquivo).
- `LocalFileStorageService`: valida extensão/mimetype (PDF, JPG, PNG) e tamanho máximo (5MB); gera nome único (`Guid + extensão`); salva em caminho configurável (`/app/storage/atestados/{ano}/{mes}/{arquivo}`), montado como volume Docker.
- Download: `GET /justificativas/{id}/anexo` retorna `FileStreamResult`.
- Abstração via interface permite trocar para S3/Azure Blob no futuro.

### 4.4 Tratamento de erros

- Middleware global `ExceptionHandlingMiddleware` mapeia exceções customizadas (`NotFoundException` → 404, `ConflictException` → 409, `ValidationException` → 400) para `ProblemDetails` (RFC 7807).
- Log via `ILogger<T>` (Serilog opcional).
- DTOs específicos por operação — nunca expor entidades EF diretamente.

### 4.5 Autenticação JWT (sem RBAC)

- `AuthController.Login` valida contra `Usuario` (hash BCrypt/`PasswordHasher<T>`).
- JWT com claims mínimas: `sub`, `email`, `name`, `exp`.
- `[Authorize]` global em todos os Controllers exceto Auth.
- Token no frontend via localStorage + interceptor Axios; expiração ~8h (cobre o expediente); refresh token fica como evolução futura.

---

## 5. Docker Compose

### 5.1 Serviços

```yaml
services:
  db:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: sistema_controle
      POSTGRES_USER: sistema_controle_user
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    ports:
      - "5432:5432"
    volumes:
      - db-data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U sistema_controle_user"]
      interval: 5s
      retries: 5

  api:
    build:
      context: ./backend
      dockerfile: Dockerfile
    depends_on:
      db:
        condition: service_healthy
    environment:
      ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-Production}
      ConnectionStrings__DefaultConnection: "Host=db;Port=5432;Database=sistema_controle;Username=sistema_controle_user;Password=${DB_PASSWORD}"
      Jwt__Secret: ${JWT_SECRET}
      Jwt__Issuer: sistema-controle-api
      Jwt__ExpirationHours: 8
      Storage__BasePath: /app/storage
    ports:
      - "5000:8080"
    volumes:
      - atestados-storage:/app/storage

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    depends_on:
      - api
    environment:
      VITE_API_URL: http://localhost:5000/api/v1
    ports:
      - "3000:80"

volumes:
  db-data:
  atestados-storage:
```

### 5.2 Variáveis de ambiente (`.env.example`)

```
DB_PASSWORD=changeme
JWT_SECRET=uma-chave-secreta-bem-longa-e-aleatoria
ASPNETCORE_ENVIRONMENT=Development
VITE_API_URL=http://localhost:5000/api/v1
```

### 5.3 Notas de ambiente

- **Dev**: `docker-compose.override.yml` monta código-fonte como volume + `dotnet watch` no backend e `vite dev` no frontend (hot reload).
- **Produção**: build multi-stage (backend: `sdk` → `aspnet` runtime; frontend: `node` → `nginx`).
- Migrations aplicadas automaticamente no startup em Dev (`db.Database.Migrate()`); em produção, considerar step explícito.

---

## 6. Roadmap de Implementação

### Fase 0 — Fundação
1. Criar solução .NET e os 4 projetos (`Api`, `Application`, `Domain`, `Infrastructure`) com referências corretas.
2. `docker-compose.yml` básico só com `db` para já ter banco em dev.
3. `.gitignore`, `.editorconfig`, `Directory.Build.props` (nullable enable, langversion).

### Fase 1 — Domínio e Persistência
4. Modelar entidades em `Domain/Entities` + enums.
5. `AppDbContext` em `Infrastructure/Data` com `DbSet<T>` por entidade.
6. `IEntityTypeConfiguration<T>` por entidade (chaves, índices únicos, relacionamentos).
7. Migration inicial (`dotnet ef migrations add InitialCreate`) aplicada no Postgres do compose.
8. `DbSeeder` com um usuário pedagogo inicial.

### Fase 2 — Autenticação
9. `AuthService` (hash de senha, geração de JWT).
10. `AddAuthentication`/`AddJwtBearer` no `Program.cs`; `[Authorize]` global.
11. `POST /auth/login` funcional.

### Fase 3 — CRUDs Base (Professores, Turmas, Salas)
12. Repositórios correspondentes.
13. Services + DTOs + Validators.
14. Controllers REST.
15. Testes unitários básicos.

### Fase 4 — Cronograma (núcleo do sistema)
16. `AulaAgendadaRepository` com queries de sobreposição (por sala e por professor).
17. `CronogramaService`: criação com checagem de conflito, edição, remoção.
18. Endpoints `grade-semanal`, `professor/{id}`, `turma/{id}`.
19. Filtros cruzados `GET /professores/{id}/turmas` e `GET /turmas/{id}/professores`.

### Fase 5 — Frequência e Ausências
20. `RegistroFrequenciaRepository` + `FrequenciaService`.
21. `GET /frequencia/dia/{data}`: cruza `AulaAgendada` (pelo `DiaSemana` da data) com `RegistroFrequencia` existente.
22. `POST /frequencia`: registra status para uma ocorrência específica.
23. Lógica de "turmas afetadas": `AulaAgendada` do professor no `DiaSemana` cujo `RegistroFrequencia` da data esteja `Ausente`.
24. Relatórios semanal/mensal (agregações por status/professor/período).

### Fase 6 — Justificativas e Upload
25. `IArquivoStorageService` + `LocalFileStorageService` (validação, nome único, persistência).
26. `JustificativaService`/`JustificativaRepository` (vincula a `RegistroFrequencia` `Ausente`).
27. Endpoints de upload e download/stream do anexo.
28. Volume `atestados-storage` no compose.

### Fase 7 — Padronização e Robustez
29. `ExceptionHandlingMiddleware` + exceções customizadas + `ProblemDetails`.
30. Revisão dos DTOs de resposta (evitar overexposure/ciclos).
31. Testes de integração (WebApplicationFactory + Testcontainers): cronograma sem/com conflito (409), registrar ausência → turma afetada.

### Fase 8 — Frontend
32. Scaffold Vite + React + TS; `react-router-dom`; Axios com interceptor JWT; `AuthContext`.
33. Tela de Login.
34. CRUDs: Professores, Turmas, Salas.
35. Tela de Cronograma: grade semanal + formulário com feedback de conflito (409).
36. Tela de Registro de Frequência (via `GET /frequencia/dia/{data}`).
37. Tela de Frequência Semanal/Mensal.
38. Tela de Turmas Afetadas (professor + data).
39. Tela de Justificativa com upload.
40. `FiltroProfessorTurma` reutilizável.

### Fase 9 — Integração Final e Deploy
41. `docker-compose.yml` completo (api + db + frontend).
42. Dockerfiles multi-stage.
43. Teste end-to-end manual do fluxo completo.
44. Atualizar `README.md` com instruções de setup definitivas.

---

## 7. Refinamentos de Frontend [implementado]

Ajustes feitos após o MVP inicial, a partir de uso real do sistema:

- **Cronograma com 3 visões** (`CronogramaPage`): seletor "Cronograma geral / Por professor
  / Por sala". As visões por professor/sala reaproveitam `GET /cronograma?professorId=` e
  `?salaId=`, agrupando o resultado por dia da semana no cliente (`agruparPorDia`); a visão
  geral usa `GET /cronograma/grade-semanal` (já agrupada pelo backend).
- **Layout de app fixo com scroll interno**: `.app-shell` ocupa `100vh` com `overflow:
  hidden`; apenas `.page` rola internamente. Isso mantém a sidebar sempre visível, e a
  grade semanal do cronograma fica dentro de um container próprio com scroll
  (`.grade-container`, `max-height: 62vh`) e cabeçalho do dia fixo (`position: sticky`).
- **Resumo semanal de turmas afetadas** (`TurmasAfetadasPage`): além do detalhe do dia
  selecionado, calcula a semana letiva (Segunda–Sexta) que contém a data escolhida e busca
  `GET /relatorios/turmas-afetadas?dataInicio=&dataFim=` para mostrar quantas turmas
  distintas e quantas ocorrências de ausência há na semana, com contagem por turma. Também
  ganhou filtro opcional por professor.
- **Data padrão "dia letivo mais próximo"** (`utils/date.ts::proximoDiaLetivo`): se hoje é
  sábado/domingo, as telas de Frequência e Turmas Afetadas abrem já na próxima segunda —
  evita a tela parecer "sem dados" só porque a grade não cobre fim de semana.
- **Persistência de filtros em `localStorage`** (`hooks/usePersistedState.ts`): visão e
  filtros do Cronograma, data da Frequência do dia, e data/professor de Turmas Afetadas
  sobrevivem a um reload da página (por navegador/dispositivo, não sincronizado entre
  usuários).
- **Dados de exemplo**: uma agenda escolar real (35 professores, 24 turmas/salas, 663
  aulas) foi importada via [`scripts/import-agenda.mjs`](../scripts/import-agenda.mjs) e
  está disponível como dump SQL em [`seed-data/`](../seed-data/) para restaurar em qualquer
  ambiente. 70 aulas da fonte original foram corretamente rejeitadas por conflito de
  horário (mesma turma com dois professores no mesmo slot) — ver
  [`seed-data/README.md`](../seed-data/README.md).

---

## Decisões-Chave

- **Cronograma como tabela associativa rica** (`AulaAgendada`) em vez de N:N simples — representa corretamente Professor+Turma+Sala+Dia+Horário com checagem de conflitos.
- **Separação entre slot recorrente (`AulaAgendada`) e ocorrência concreta (`RegistroFrequencia`)** — decisão de design mais importante do domínio: permite calcular frequência histórica por data real e detectar turmas afetadas em um dia específico.
- **Armazenamento local em volume Docker** para atestados no MVP, abstraído por interface para migração futura a object storage.
- **Autenticação simples**: JWT sem RBAC, alinhado ao perfil único (pedagogo).
- **Camadas pragmáticas** (Api/Application/Domain/Infrastructure) sem CQRS/MediatR.
