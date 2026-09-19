import { useEffect, useState } from 'react';
import { turmaService } from '../../services/turmaService';
import { extractErrorMessage } from '../../services/api';
import type { Turma, TurmaInput, Turno } from '../../types/turma';

const VAZIO: TurmaInput = { nome: '', turno: 'Manha', anoLetivo: new Date().getFullYear() };

export function TurmasPage() {
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [form, setForm] = useState<TurmaInput>(VAZIO);
  const [editandoId, setEditandoId] = useState<number | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function carregar() {
    try {
      setTurmas(await turmaService.list());
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  function iniciarEdicao(turma: Turma) {
    setEditandoId(turma.id);
    setForm({ nome: turma.nome, turno: turma.turno, anoLetivo: turma.anoLetivo, ativo: turma.ativo });
  }

  function cancelarEdicao() {
    setEditandoId(null);
    setForm(VAZIO);
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setErro(null);
    setCarregando(true);
    try {
      if (editandoId) {
        await turmaService.update(editandoId, { ...form, ativo: form.ativo ?? true });
      } else {
        await turmaService.create(form);
      }
      cancelarEdicao();
      await carregar();
    } catch (error) {
      setErro(extractErrorMessage(error));
    } finally {
      setCarregando(false);
    }
  }

  async function handleInativar(id: number) {
    if (!confirm('Inativar esta turma?')) return;
    try {
      await turmaService.inativar(id);
      await carregar();
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  return (
    <div>
      <h1>Turmas</h1>

      <form className="card" onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="field">
            <label>Nome</label>
            <input value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
          </div>
          <div className="field">
            <label>Turno</label>
            <select value={form.turno} onChange={(e) => setForm({ ...form, turno: e.target.value as Turno })}>
              <option value="Manha">Manhã</option>
              <option value="Tarde">Tarde</option>
              <option value="Noite">Noite</option>
            </select>
          </div>
          <div className="field">
            <label>Ano letivo</label>
            <input
              type="number"
              value={form.anoLetivo}
              onChange={(e) => setForm({ ...form, anoLetivo: Number(e.target.value) })}
              required
            />
          </div>
          {editandoId && (
            <div className="field">
              <label>Status</label>
              <select value={form.ativo ? 'true' : 'false'} onChange={(e) => setForm({ ...form, ativo: e.target.value === 'true' })}>
                <option value="true">Ativa</option>
                <option value="false">Inativa</option>
              </select>
            </div>
          )}
        </div>
        {erro && <p className="error-message">{erro}</p>}
        <div style={{ display: 'flex', gap: '0.6rem' }}>
          <button type="submit" className="btn btn--primary" disabled={carregando}>
            {editandoId ? 'Salvar alterações' : 'Cadastrar turma'}
          </button>
          {editandoId && (
            <button type="button" className="btn" onClick={cancelarEdicao}>
              Cancelar
            </button>
          )}
        </div>
      </form>

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Nome</th>
              <th>Turno</th>
              <th>Ano letivo</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {turmas.map((turma) => (
              <tr key={turma.id}>
                <td>{turma.nome}</td>
                <td>{turma.turno}</td>
                <td>{turma.anoLetivo}</td>
                <td>
                  <span className={`badge ${turma.ativo ? 'badge--success' : 'badge--muted'}`}>
                    {turma.ativo ? 'Ativa' : 'Inativa'}
                  </span>
                </td>
                <td style={{ display: 'flex', gap: '0.4rem' }}>
                  <button className="btn" onClick={() => iniciarEdicao(turma)}>
                    Editar
                  </button>
                  <button className="btn btn--danger" onClick={() => handleInativar(turma.id)}>
                    Inativar
                  </button>
                </td>
              </tr>
            ))}
            {turmas.length === 0 && (
              <tr>
                <td colSpan={5}>Nenhuma turma encontrada.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
