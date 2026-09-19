import { useEffect, useState } from 'react';
import { salaService } from '../../services/salaService';
import { extractErrorMessage } from '../../services/api';
import type { Sala, SalaInput } from '../../types/sala';

const VAZIO: SalaInput = { nome: '', capacidade: undefined };

export function SalasPage() {
  const [salas, setSalas] = useState<Sala[]>([]);
  const [form, setForm] = useState<SalaInput>(VAZIO);
  const [editandoId, setEditandoId] = useState<number | null>(null);
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function carregar() {
    try {
      setSalas(await salaService.list());
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  function iniciarEdicao(sala: Sala) {
    setEditandoId(sala.id);
    setForm({ nome: sala.nome, capacidade: sala.capacidade ?? undefined, ativo: sala.ativo });
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
        await salaService.update(editandoId, { ...form, ativo: form.ativo ?? true });
      } else {
        await salaService.create(form);
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
    if (!confirm('Inativar esta sala?')) return;
    try {
      await salaService.inativar(id);
      await carregar();
    } catch (error) {
      setErro(extractErrorMessage(error));
    }
  }

  return (
    <div>
      <h1>Salas</h1>

      <form className="card" onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="field">
            <label>Nome</label>
            <input value={form.nome} onChange={(e) => setForm({ ...form, nome: e.target.value })} required />
          </div>
          <div className="field">
            <label>Capacidade</label>
            <input
              type="number"
              value={form.capacidade ?? ''}
              onChange={(e) => setForm({ ...form, capacidade: e.target.value ? Number(e.target.value) : undefined })}
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
            {editandoId ? 'Salvar alterações' : 'Cadastrar sala'}
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
              <th>Capacidade</th>
              <th>Status</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {salas.map((sala) => (
              <tr key={sala.id}>
                <td>{sala.nome}</td>
                <td>{sala.capacidade ?? '—'}</td>
                <td>
                  <span className={`badge ${sala.ativo ? 'badge--success' : 'badge--muted'}`}>
                    {sala.ativo ? 'Ativa' : 'Inativa'}
                  </span>
                </td>
                <td style={{ display: 'flex', gap: '0.4rem' }}>
                  <button className="btn" onClick={() => iniciarEdicao(sala)}>
                    Editar
                  </button>
                  <button className="btn btn--danger" onClick={() => handleInativar(sala.id)}>
                    Inativar
                  </button>
                </td>
              </tr>
            ))}
            {salas.length === 0 && (
              <tr>
                <td colSpan={4}>Nenhuma sala encontrada.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
