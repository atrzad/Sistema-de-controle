# Scripts

## `import-agenda.mjs`

Importa uma agenda escolar de exemplo (35 professores, 24 turmas/salas, ~730 aulas
agendadas — 663 criadas com sucesso, 70 rejeitadas por conflito de horário legítimo
detectado na fonte original) diretamente via chamadas HTTP à API.

Requer:
- Stack rodando (`docker compose up -d`); por padrão o script fala com a API pelo proxy do
  frontend em `http://localhost:3000/api/v1` (sobrescreva com `API_URL=...`)
- Node.js 18+ (usa `fetch` nativo)
- Credenciais do usuário inicial — as mesmas de `ADMIN_EMAIL`/`ADMIN_PASSWORD` do `.env`

```bash
ADMIN_EMAIL=pedagogo@sistemadecontrole.local ADMIN_PASSWORD='sua-senha' node scripts/import-agenda.mjs
```

O script é idempotente na criação de professores/turmas/salas (se já existirem por
email/nome, reaproveita o registro existente em vez de duplicar); aulas agendadas
conflitantes são apenas logadas e puladas, não interrompem a importação.

Para uma alternativa mais rápida (sem precisar da API rodando durante a importação),
veja o dump SQL em [`../seed-data/`](../seed-data/).
