import { useEffect, useState } from 'react';
import { cronogramaService } from '../../services/cronogramaService';
import { professorService } from '../../services/professorService';
import { turmaService } from '../../services/turmaService';
import { salaService } from '../../services/salaService';
import { extractErrorMessage } from '../../services/api';
import { usePersistedState } from '../../hooks/usePersistedState';
import {
  DIAS_SEMANA,
  DIA_SEMANA_LABEL,
  type AulaAgendada,
  type AulaAgendadaInput,
  type DiaSemana,
  type GradeDia,
} from '../../types/cronograma';
import type { Professor } from '../../types/professor';
import type { Turma } from '../../types/turma';
import type { Sala } from '../../types/sala';

const VAZIO: AulaAgendadaInput = {
  professorId: 0,
  turmaId: 0,
  salaId: 0,
  diaSemana: 'Segunda',
  horaInicio: '08:00',
  horaFim: '09:00',
  disciplina: '',
};

type Visao = 'geral' | 'professor' | 'sala';

function agruparPorDia(aulas: AulaAgendada[]): GradeDia[] {
  return DIAS_SEMANA.map((dia) => ({
    diaSemana: dia,
    aulas: aulas.filter((a) => a.diaSemana === dia).sort((a, b) => a.horaInicio.localeCompare(b.horaInicio)),
  }));
}

export function CronogramaPage() {
  const [grade, setGrade] = useState<GradeDia[]>([]);
  const [professores, setProfessores] = useState<Professor[]>([]);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [salas, setSalas] = useState<Sala[]>([]);
  const [form, setForm] = useState<AulaAgendadaInput>(VAZIO);
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  const [visao, setVisao] = usePersistedState<Visao>('cronograma:visao', 'geral');
  const [filtroProfessorId, setFiltroProfessorId] = usePersistedState('cronograma:filtroProfessorId', 0);
  const [filtroSalaId, setFiltroSalaId] = usePersistedState('cronograma:filtroSalaId', 0);

  async function carregarGrade() {
    try {
      if (visao === 'professor') {
        if (!filtroProfessorId) {
          setGrade(agruparPorDia([]));
          return;
        }
        setGrade(agruparPorDia(await cronogramaService.list({ professorId: filtroProfessorId })));
      } else if (visao === 'sala') {
        if (!filtroSalaId) {
          setGrade(agruparPorDia([]));
          return;
        }
        setGrade(agruparPorDia(await cronogramaService.list({ salaId: filtroSalaId })));
      } else {
        setGrade(await cronogramaService.getGradeSemanal());
      }
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  useEffect(() => {
    carregarGrade();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [visao, filtroProfessorId, filtroSalaId]);

  useEffect(() => {
    professorService.list({ ativo: true }).then(setProfessores).catch(() => {});
    turmaService.list({ ativo: true }).then(setTurmas).catch(() => {});
    salaService.list({ ativo: true }).then(setSalas).catch(() => {});
  }, []);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setErro(null);

    if (!form.professorId || !form.turmaId || !form.salaId) {
      setErro('Selecione professor, turma e sala.');
      return;
    }

    setCarregando(true);
    try {
      await cronogramaService.create(form);
      setForm(VAZIO);
      await carregarGrade();
    } catch (error) {
      setErro(extractErrorMessage(error));
    } finally {
      setCarregando(false);
    }
  }

  async function handleRemover(id: number) {
    if (!confirm('Remover esta aula do cronograma?')) return;
    try {
      await cronogramaService.inativar(id);
      await carregarGrade();
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  const gradePorDia = new Map(grade.map((g) => [g.diaSemana, g.aulas]));

  return (
    <div>
      <h1>Cronograma semanal</h1>

      <form className="card" onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="field">
            <label>Professor</label>
            <select value={form.professorId} onChange={(e) => setForm({ ...form, professorId: Number(e.target.value) })}>
              <option value={0}>Selecione...</option>
              {professores.map((p) => (
                <option key={p.id} value={p.id}>{p.nome}</option>
              ))}
            </select>
          </div>
          <div className="field">
            <label>Turma</label>
            <select value={form.turmaId} onChange={(e) => setForm({ ...form, turmaId: Number(e.target.value) })}>
              <option value={0}>Selecione...</option>
              {turmas.map((t) => (
                <option key={t.id} value={t.id}>{t.nome}</option>
              ))}
            </select>
          </div>
          <div className="field">
            <label>Sala</label>
            <select value={form.salaId} onChange={(e) => setForm({ ...form, salaId: Number(e.target.value) })}>
              <option value={0}>Selecione...</option>
              {salas.map((s) => (
                <option key={s.id} value={s.id}>{s.nome}</option>
              ))}
            </select>
          </div>
          <div className="field">
            <label>Dia da semana</label>
            <select value={form.diaSemana} onChange={(e) => setForm({ ...form, diaSemana: e.target.value as DiaSemana })}>
              {DIAS_SEMANA.map((dia) => (
                <option key={dia} value={dia}>{DIA_SEMANA_LABEL[dia]}</option>
              ))}
            </select>
          </div>
          <div className="field">
            <label>Início</label>
            <input type="time" value={form.horaInicio} onChange={(e) => setForm({ ...form, horaInicio: e.target.value })} />
          </div>
          <div className="field">
            <label>Fim</label>
            <input type="time" value={form.horaFim} onChange={(e) => setForm({ ...form, horaFim: e.target.value })} />
          </div>
          <div className="field">
            <label>Disciplina (opcional)</label>
            <input value={form.disciplina} onChange={(e) => setForm({ ...form, disciplina: e.target.value })} />
          </div>
        </div>
        {erro && <p className="error-message">{erro}</p>}
        <button type="submit" className="btn btn--primary" disabled={carregando}>
          Adicionar ao cronograma
        </button>
      </form>

      <div className="toolbar">
        <div className="field" style={{ minWidth: 180 }}>
          <label>Visão</label>
          <select value={visao} onChange={(e) => setVisao(e.target.value as Visao)}>
            <option value="geral">Cronograma geral</option>
            <option value="professor">Por professor</option>
            <option value="sala">Por sala</option>
          </select>
        </div>

        {visao === 'professor' && (
          <div className="field" style={{ minWidth: 220 }}>
            <label>Professor</label>
            <select value={filtroProfessorId} onChange={(e) => setFiltroProfessorId(Number(e.target.value))}>
              <option value={0}>Selecione um professor...</option>
              {professores.map((p) => (
                <option key={p.id} value={p.id}>{p.nome}</option>
              ))}
            </select>
          </div>
        )}

        {visao === 'sala' && (
          <div className="field" style={{ minWidth: 220 }}>
            <label>Sala</label>
            <select value={filtroSalaId} onChange={(e) => setFiltroSalaId(Number(e.target.value))}>
              <option value={0}>Selecione uma sala...</option>
              {salas.map((s) => (
                <option key={s.id} value={s.id}>{s.nome}</option>
              ))}
            </select>
          </div>
        )}
      </div>

      <div className="grade-container">
        <div className="grade-semanal">
          {DIAS_SEMANA.map((dia) => (
            <div key={dia} className="grade-dia">
              <h3>{DIA_SEMANA_LABEL[dia]}</h3>
              {(gradePorDia.get(dia) ?? []).map((aula) => (
                <div key={aula.id} className="aula-item">
                  <strong>{aula.horaInicio.slice(0, 5)} – {aula.horaFim.slice(0, 5)}</strong>
                  {aula.professorNome}<br />
                  {aula.turmaNome} · {aula.salaNome}
                  {aula.disciplina && <><br />{aula.disciplina}</>}
                  <div style={{ marginTop: '0.4rem' }}>
                    <button className="btn btn--danger" onClick={() => handleRemover(aula.id)}>
                      Remover
                    </button>
                  </div>
                </div>
              ))}
              {(gradePorDia.get(dia) ?? []).length === 0 && (
                <p style={{ color: 'var(--color-text-muted)', fontSize: '0.85rem' }}>
                  {visao !== 'geral' && !filtroProfessorId && !filtroSalaId ? 'Selecione um filtro acima.' : 'Sem aulas.'}
                </p>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
