#!/usr/bin/env bash
# Restaura um backup gerado por scripts/backup.sh.
#
#   scripts/restore.sh backups/db-AAAAMMDD-HHMMSS.dump backups/anexos-AAAAMMDD-HHMMSS.tar.gz
#
# ATENÇÃO: substitui os dados atuais do banco e os anexos.
set -euo pipefail

DUMP="${1:?Informe o arquivo .dump do banco}"
ANEXOS="${2:?Informe o arquivo .tar.gz dos anexos}"

read -r -p "Isso substitui o banco e os anexos atuais. Continuar? [s/N] " resposta
[[ "$resposta" == "s" || "$resposta" == "S" ]] || { echo "Cancelado."; exit 1; }

echo "Restaurando banco..."
docker compose exec -T db pg_restore -U sistema_controle_user -d sistema_controle \
  --clean --if-exists --no-owner < "$DUMP"

echo "Restaurando anexos..."
docker compose exec -T api sh -c 'find /app/storage -mindepth 1 -delete && tar -xzf - -C /app/storage' < "$ANEXOS"

echo "Reiniciando API..."
docker compose restart api

echo "Restauração concluída."
