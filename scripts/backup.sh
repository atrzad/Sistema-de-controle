#!/usr/bin/env bash
# Backup do banco (pg_dump) e dos anexos de atestado (volume atestados-storage).
#
# Uso (na raiz do projeto, onde fica o docker-compose.yml):
#   scripts/backup.sh [diretório-destino]      # padrão: ./backups
#
# Agendamento sugerido na VPS (crontab -e), todo dia às 02:30:
#   30 2 * * * cd /caminho/Sistema-de-controle && scripts/backup.sh /var/backups/sistema-controle >> /var/log/sistema-controle-backup.log 2>&1
#
# Mantém os últimos RETENCAO_DIAS dias (padrão 14). Copie os backups também para fora
# do servidor (outro host, bucket S3/B2, etc.) — backup no mesmo disco não protege contra
# perda do servidor.
set -euo pipefail

DESTINO="${1:-./backups}"
RETENCAO_DIAS="${RETENCAO_DIAS:-14}"
CARIMBO="$(date +%Y%m%d-%H%M%S)"

mkdir -p "$DESTINO"

echo "[$(date -Is)] Gerando dump do banco..."
docker compose exec -T db pg_dump -U sistema_controle_user -d sistema_controle --format=custom \
  > "$DESTINO/db-$CARIMBO.dump"

echo "[$(date -Is)] Compactando anexos..."
docker compose exec -T api tar -czf - -C /app/storage . > "$DESTINO/anexos-$CARIMBO.tar.gz"

find "$DESTINO" -maxdepth 1 -type f \( -name 'db-*.dump' -o -name 'anexos-*.tar.gz' \) \
  -mtime +"$RETENCAO_DIAS" -delete

echo "[$(date -Is)] Backup concluído em $DESTINO (db-$CARIMBO.dump, anexos-$CARIMBO.tar.gz)"
