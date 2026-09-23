#!/usr/bin/env bash
# Gera o pacote desktop do Sistema de Controle: executável único + PostgreSQL portátil +
# frontend, compactados em um .zip pronto para distribuir.
#
#   scripts/build-desktop.sh            # Windows (win-x64) -> dist/SistemaDeControle-win-x64.zip
#   scripts/build-desktop.sh linux-x64  # Linux, útil para testar o pacote sem Windows
#
# Requer: .NET 8 SDK, Node.js 20+, python3 e curl. Pode ser rodado em Linux, macOS ou WSL
# (o .exe é compilado de forma cruzada, não precisa de Windows).
set -euo pipefail

RID="${1:-win-x64}"
PG_VERSION="${PG_VERSION:-16.15.0}"

case "$RID" in
  win-x64)   PG_PLATAFORMA="windows-amd64"; PG_TXZ="postgres-windows-x86_64.txz" ;;
  linux-x64) PG_PLATAFORMA="linux-amd64";   PG_TXZ="postgres-linux-x86_64.txz" ;;
  *) echo "RID não suportado: $RID (use win-x64 ou linux-x64)" >&2; exit 1 ;;
esac

RAIZ="$(cd "$(dirname "$0")/.." && pwd)"
CACHE="$RAIZ/.cache/desktop"
SAIDA="$RAIZ/dist/SistemaDeControle-$RID"
ZIP="$RAIZ/dist/SistemaDeControle-$RID.zip"

mkdir -p "$CACHE"
rm -rf "$SAIDA" "$ZIP"
mkdir -p "$SAIDA"

echo ">> PostgreSQL portátil $PG_VERSION ($PG_PLATAFORMA)"
JAR="$CACHE/pg-$PG_PLATAFORMA-$PG_VERSION.jar"
if [[ ! -f "$JAR" ]]; then
  curl -fL --retry 3 -o "$JAR" \
    "https://repo1.maven.org/maven2/io/zonky/test/postgres/embedded-postgres-binaries-$PG_PLATAFORMA/$PG_VERSION/embedded-postgres-binaries-$PG_PLATAFORMA-$PG_VERSION.jar"
fi
python3 -c "import zipfile,sys; zipfile.ZipFile(sys.argv[1]).extract(sys.argv[2], sys.argv[3])" "$JAR" "$PG_TXZ" "$CACHE"
mkdir -p "$SAIDA/pgsql"
tar -xJf "$CACHE/$PG_TXZ" -C "$SAIDA/pgsql"

echo ">> Frontend"
(cd "$RAIZ/frontend" && npm ci --no-audit --no-fund && VITE_API_URL=/api/v1 npm run build)
cp -r "$RAIZ/frontend/dist" "$SAIDA/wwwroot"

echo ">> API ($RID, self-contained, arquivo único)"
dotnet publish "$RAIZ/backend/src/SistemaDeControle.Api/SistemaDeControle.Api.csproj" \
  -c Release -r "$RID" --self-contained true \
  -p:Desktop=true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -p:DebugType=none \
  -o "$SAIDA"

cp "$RAIZ/scripts/desktop/LEIA-ME.txt" "$SAIDA/"
if [[ "$RID" == win-x64 ]]; then
  cp "$RAIZ/scripts/desktop/Redefinir senha do administrador.bat" "$SAIDA/"
fi

echo ">> Compactando"
python3 - "$SAIDA" "$ZIP" <<'PY'
import os, sys, zipfile
origem, destino = sys.argv[1], sys.argv[2]
base = os.path.dirname(origem)
with zipfile.ZipFile(destino, "w", zipfile.ZIP_DEFLATED) as z:
    for pasta, _, arquivos in os.walk(origem):
        for nome in arquivos:
            caminho = os.path.join(pasta, nome)
            info = zipfile.ZipInfo.from_file(caminho, os.path.relpath(caminho, base))
            info.compress_type = zipfile.ZIP_DEFLATED
            with open(caminho, "rb") as f:
                z.writestr(info, f.read())
PY

echo
echo "Pacote gerado: $ZIP ($(du -h "$ZIP" | cut -f1))"
