import { useEffect, useState } from 'react';
import { frequenciaService } from '../../services/frequenciaService';
import { relatorioService } from '../../services/relatorioService';
import { professorService } from '../../services/professorService';
import { extractErrorMessage } from '../../services/api';
import { usePersistedState } from '../../hooks/usePersistedState';
import { proximoDiaLetivo } from '../../utils/date';
import { STATUS_LABEL, type TurmaAfetada } from '../../types/frequencia';
import type { Professor } from '../../types/professor';

/** Segunda-feira da semana que contém a data informada (semana letiva Segunda–Sexta). */
function inicioDaSemana(dataIso: string): string {
  const data = new Date(`${dataIso}T00:00:00`);
  const diasDesdeSegunda = (data.getDay() + 6) % 7;
  data.setDate(data.getDate() - diasDesdeSegunda);
  return data.toISOString().slice(0, 10);
}

function fimDaSemana(inicioIso: string): string {
  const data = new Date(`${inicioIso}T00:00:00`);
  data.setDate(data.getDate() + 4); // sexta-feira
  return data.toISOString().slice(0, 10);
}

function formatarBr(dataIso: string): string {
  const [ano, mes, dia] = dataIso.split('-');
  return `${dia}/${mes}/${ano}`;
}

export function TurmasAfetadasPage() {
  const [data, setData] = usePersistedState('turmas-afetadas:data', proximoDiaLetivo());
  const [filtroProfessorId, setFiltroProfessorId] = usePersistedState('turmas-afetadas:professorId', 0);

  const [professores, setProfessores] = useState<Professor[]>([]);
  const [turmasAfetadas, setTurmasAfetadas] = useState<TurmaAfetada[]>([]);
  const [turmasAfetadasSemana, setTurmasAfetadasSemana] = useState<TurmaAfetada[]>([]);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    professorService.list({ ativo: true }).then(setProfessores).catch(() => {});
  }, []);

  useEffect(() => {
    frequenciaService
      .getTurmasAfetadas(data, filtroProfessorId || undefined)
      .then(setTurmasAfetadas)
      .catch((error) => setErro(extractErrorMessage(error)));
  }, [data, filtroProfessorId]);

  const semanaInicio = inicioDaSemana(data);
  const semanaFim = fimDaSemana(semanaInicio);

  useEffect(() => {
    relatorioService
      .getTurmasAfetadasHistorico(semanaInicio, semanaFim)
      .then(setTurmasAfetadasSemana)
      .catch((error) => setErro(extractErrorMessage(error)));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [semanaInicio, semanaFim]);

  const semanaFiltrada = filtroProfessorId
    ? turmasAfetadasSemana.filter((t) => t.professorId === filtroProfessorId)
    : turmasAfetadasSemana;

  const turmasDistintasSemana = new Set(semanaFiltrada.map((t) => t.turmaId));
  const contagemPorTurma = new Map<string, number>();
  for (const t of semanaFiltrada) {
    contagemPorTurma.set(t.turmaNome, (contagemPorTurma.get(t.turmaNome) ?? 0) + 1);
  }

  return (
    <div>
      <h1>Turmas afetadas por ausência</h1>

      <div className="toolbar">
        <label>
          Data:{' '}
          <input type="date" value={data} onChange={(e) => setData(e.target.value)} />
        </label>
        <div className="field" style={{ minWidth: 220 }}>
          <label>Professor (opcional)</label>
          <select value={filtroProfessorId} onChange={(e) => setFiltroProfessorId(Number(e.target.value))}>
            <option value={0}>Todos os professores</option>
            {professores.map((p) => (
              <option key={p.id} value={p.id}>{p.nome}</option>
            ))}
          </select>
        </div>
      </div>

      {erro && <p className="error-message">{erro}</p>}

      <div className="card">
        <h3 style={{ marginTop: 0 }}>Resumo da semana ({formatarBr(semanaInicio)} – {formatarBr(semanaFim)})</h3>
        <div style={{ display: 'flex', gap: '2rem', flexWrap: 'wrap', marginBottom: turmasDistintasSemana.size > 0 ? '1rem' : 0 }}>
          <div>
            <div style={{ fontSize: '1.8rem', fontWeight: 700 }}>{turmasDistintasSemana.size}</div>
            <div style={{ color: 'var(--color-text-muted)', fontSize: '0.85rem' }}>turma(s) distinta(s) afetada(s)</div>
          </div>
          <div>
            <div style={{ fontSize: '1.8rem', fontWeight: 700 }}>{semanaFiltrada.length}</div>
            <div style={{ color: 'var(--color-text-muted)', fontSize: '0.85rem' }}>ocorrência(s) de ausência na semana</div>
          </div>
        </div>

        {contagemPorTurma.size > 0 && (
          <table>
            <thead>
              <tr>
                <th>Turma</th>
                <th>Aulas afetadas na semana</th>
              </tr>
            </thead>
            <tbody>
              {[...contagemPorTurma.entries()].sort((a, b) => b[1] - a[1]).map(([turma, qtd]) => (
                <tr key={turma}>
                  <td>{turma}</td>
                  <td>{qtd}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="card">
        <h3 style={{ marginTop: 0 }}>Detalhe do dia {formatarBr(data)}</h3>
        <table>
          <thead>
            <tr>
              <th>Horário</th>
              <th>Turma</th>
              <th>Sala</th>
              <th>Professor ausente</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {turmasAfetadas.map((t, index) => (
              <tr key={index}>
                <td>{t.horaInicio.slice(0, 5)} – {t.horaFim.slice(0, 5)}</td>
                <td>{t.turmaNome}</td>
                <td>{t.salaNome}</td>
                <td>{t.professorNome}</td>
                <td>
                  <span className={`badge ${t.status === 'AusenciaJustificada' ? 'badge--warning' : 'badge--danger'}`}>
                    {STATUS_LABEL[t.status]}
                  </span>
                </td>
              </tr>
            ))}
            {turmasAfetadas.length === 0 && (
              <tr>
                <td colSpan={5}>Nenhuma turma afetada nesta data{filtroProfessorId ? ' para o professor selecionado' : ''}.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
