import { useState, type FormEvent } from 'react';
import { authService } from '../../services/authService';
import { extractErrorMessage } from '../../services/api';

export function AlterarSenhaPage() {
  const [senhaAtual, setSenhaAtual] = useState('');
  const [novaSenha, setNovaSenha] = useState('');
  const [confirmacao, setConfirmacao] = useState('');
  const [erro, setErro] = useState<string | null>(null);
  const [sucesso, setSucesso] = useState(false);
  const [carregando, setCarregando] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);
    setSucesso(false);

    if (novaSenha !== confirmacao) {
      setErro('A confirmação não confere com a nova senha.');
      return;
    }

    setCarregando(true);
    try {
      await authService.alterarSenha(senhaAtual, novaSenha);
      setSenhaAtual('');
      setNovaSenha('');
      setConfirmacao('');
      setSucesso(true);
    } catch (error) {
      setErro(extractErrorMessage(error));
    } finally {
      setCarregando(false);
    }
  }

  return (
    <div>
      <h1>Alterar senha</h1>

      <form className="card" onSubmit={handleSubmit} style={{ maxWidth: '28rem' }}>
        <div className="field">
          <label htmlFor="senhaAtual">Senha atual</label>
          <input id="senhaAtual" type="password" autoComplete="current-password" value={senhaAtual} onChange={(e) => setSenhaAtual(e.target.value)} required />
        </div>
        <div className="field" style={{ marginTop: '0.75rem' }}>
          <label htmlFor="novaSenha">Nova senha</label>
          <input id="novaSenha" type="password" autoComplete="new-password" minLength={10} value={novaSenha} onChange={(e) => setNovaSenha(e.target.value)} required />
        </div>
        <div className="field" style={{ marginTop: '0.75rem' }}>
          <label htmlFor="confirmacao">Confirmar nova senha</label>
          <input id="confirmacao" type="password" autoComplete="new-password" minLength={10} value={confirmacao} onChange={(e) => setConfirmacao(e.target.value)} required />
        </div>
        <p style={{ fontSize: '0.85rem', marginTop: '0.75rem' }}>Mínimo de 10 caracteres, com letras e números.</p>
        {erro && <p className="error-message">{erro}</p>}
        {sucesso && <p><span className="badge badge--success">Senha alterada com sucesso.</span></p>}
        <button type="submit" className="btn btn--primary" style={{ marginTop: '0.5rem' }} disabled={carregando}>
          {carregando ? 'Salvando...' : 'Alterar senha'}
        </button>
      </form>
    </div>
  );
}
