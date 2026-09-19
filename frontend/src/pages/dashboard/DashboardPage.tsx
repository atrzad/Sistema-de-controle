import { useEffect, useState } from 'react';
import { relatorioService } from '../../services/relatorioService';
import { extractErrorMessage } from '../../services/api';
import type { RelatorioFrequencia } from '../../types/relatorio';

function primeiroDiaDoMes(): string {
  const now = new Date();
  return new Date(now.getFullYear(), now.getMonth(), 1).toISOString().slice(0, 10);
}

function hoje(): string {
  return new Date().toISOString().slice(0, 10);
}

export function DashboardPage() {
  const [dataInicio, setDataInicio] = useState(primeiroDiaDoMes());
  const [dataFim, setDataFim] = useState(hoje());
  const [relatorios, setRelatorios] = useState<RelatorioFrequencia[]>([]);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    relatorioService
      .getConsolidado(dataInicio, dataFim)
      .then(setRelatorios)
      .catch((error) => setErro(extractErrorMessage(error)));
  }, [dataInicio, dataFim]);

  return (
    <div>
      <h1>Dashboard — Frequência consolidada</h1>

      <div className="toolbar">
        <label>
          De: <input type="date" value={dataInicio} onChange={(e) => setDataInicio(e.target.value)} />
        </label>
        <label>
          Até: <input type="date" value={dataFim} onChange={(e) => setDataFim(e.target.value)} />
        </label>
      </div>

      {erro && <p className="error-message">{erro}</p>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Professor</th>
              <th>Total de aulas</th>
              <th>Presenças</th>
              <th>Ausências</th>
              <th>Ausências justificadas</th>
              <th>% presença</th>
            </tr>
          </thead>
          <tbody>
            {relatorios.map((r) => (
              <tr key={r.professorId}>
                <td>{r.professorNome}</td>
                <td>{r.totalAulas}</td>
                <td>{r.totalPresencas}</td>
                <td>{r.totalAusencias}</td>
                <td>{r.totalAusenciasJustificadas}</td>
                <td>
                  <span className={`badge ${r.percentualPresenca >= 90 ? 'badge--success' : r.percentualPresenca >= 75 ? 'badge--warning' : 'badge--danger'}`}>
                    {r.percentualPresenca.toFixed(1)}%
                  </span>
                </td>
              </tr>
            ))}
            {relatorios.length === 0 && (
              <tr>
                <td colSpan={6}>Nenhum dado no período selecionado.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
