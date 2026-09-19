import { useEffect, useState } from 'react';
import { professorService } from '../../services/professorService';
import { extractErrorMessage } from '../../services/api';
import type { Professor, ProfessorInput } from '../../types/professor';

const VAZIO: ProfessorInput = { nome: '', email: '', telefone: '', matricula: '', disciplina: '' };

export function ProfessoresPage() {
  const [professores, setProfessores] = useState<Professor[]>([]);
  const [form, setForm] = useState<ProfessorInput>(VAZIO);
  const [editandoId, setEditandoId] = useState<number | null>(null);
  const [filtroNome, setFiltroNome] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function carregar() {
    try {
      const dados = await professorService.list(filtroNome ? { nome: filtroNome } : undefined);
      setProfessores(dados);
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  useEffect(() => {
    carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [filtroNome]);

  function iniciarEdicao(professor: Professor) {
    setEditandoId(professor.id);
    setForm({
      nome: professor.nome,
      email: professor.email,
      telefone: professor.telefone ?? '',
      matricula: professor.matricula,
      disciplina: professor.disciplina ?? '',
      ativo: professor.ativo,
    });
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
        await professorService.update(editandoId, { ...form, ativo: form.ativo ?? true });
      } else {
        await professorService.create(form);
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
    if (!confirm('Inativar este professor?')) return;
    try {
      await professorService.inativar(id);
      await carregar();
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  return (
    <div>
      <h1>Professores</h1>

      <form className="card" onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="field">
            <label>Nome</label>
            <input value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
          </div>
          <div className="field">
            <label>E-mail</label>
            <input type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
          </div>
          <div className="field">
            <label>Telefone</label>
            <input value={form.telefone} onChange={(e) => setForm({ ...form, telefone: e.target.value })} />
          </div>
          <div className="field">
            <label>Matrícula</label>
            <input value={form.matricula} onChange={(e) => setForm({ ...form, matricula: e.target.value })} required />
          </div>
          <div className="field">
            <label>Disciplina</label>
            <input value={form.disciplina} onChange={(e) => setForm({ ...form, disciplina: e.target.value })} />
          </div>
          {editandoId && (
            <div className="field">
              <label>Status</label>
              <select
                value={form.ativo ? 'true' : 'false'}
                onChange={(e) => setForm({ ...form, ativo: e.target.value === 'true' })}
              >
                <option value="true">Ativo</option>
                <option value="false">Inativo</option>
              </select>
            </div>
          )}
        </div>
        {erro && <p className="error-message">{erro}</p>}
        <div style={{ display: 'flex', gap: '0.6rem' }}>
          <button type="submit" className="btn btn--primary" disabled={carregando}>
            {editandoId ? 'Salvar alterações' : 'Cadastrar professor'}
          </button>
          {editandoId && (
            <button type="button" className="btn" onClick={cancelarEdicao}>
              Cancelar
            </button>
          )}
        </div>
      </form>

      <div className="toolbar">
        <input
          placeholder="Filtrar por nome..."
          value={filtroNome}
          onChange={(e) => setFiltroNome(e.target.value)}
          style={{ padding: '0.5rem 0.6rem', borderRadius: 6, border: '1px solid var(--color-border)' }}
        />
      </div>

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Nome</th>
              <th>E-mail</th>
              <th>Matrícula</th>
              <th>Disciplina</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {professores.map((professor) => (
              <tr key={professor.id}>
                <td>{professor.nome}</td>
                <td>{professor.email}</td>
                <td>{professor.matricula}</td>
                <td>{professor.disciplina ?? '—'}</td>
                <td>
                  <span className={`badge ${professor.ativo ? 'badge--success' : 'badge--muted'}`}>
                    {professor.ativo ? 'Ativo' : 'Inativo'}
                  </span>
                </td>
                <td style={{ display: 'flex', gap: '0.4rem' }}>
                  <button className="btn" onClick={() => iniciarEdicao(professor)}>
                    Editar
                  </button>
                  <button className="btn btn--danger" onClick={() => handleInativar(professor.id)}>
                    Inativar
                  </button>
                </td>
              </tr>
            ))}
            {professores.length === 0 && (
              <tr>
                <td colSpan={6}>Nenhum professor encontrado.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
