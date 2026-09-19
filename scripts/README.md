# Scripts

## `import-agenda.mjs`

Importa uma agenda escolar de exemplo (35 professores, 24 turmas/salas, ~730 aulas
agendadas — 663 criadas com sucesso, 70 rejeitadas por conflito de horário legítimo
detectado na fonte original) diretamente via chamadas HTTP à API.

Requer:
- API rodando em `http://localhost:5000` (`docker compose up -d`)
- Node.js 18+ (usa `fetch` nativo)
- Usuário pedagogo padrão já existente (criado automaticamente pelo seed no primeiro
  start da API: `pedagogo@sistemadecontrole.local` / `TrocarSenha123!`)

```bash
node scripts/import-agenda.mjs
```

O script é idempotente na criação de professores/turmas/salas (se já existirem por
email/nome, reaproveita o registro existente em vez de duplicar); aulas agendadas
conflitantes são apenas logadas e puladas, não interrompem a importação.

Para uma alternativa mais rápida (sem precisar da API rodando durante a importação),
veja o dump SQL em [`../seed-data/`](../seed-data/).
