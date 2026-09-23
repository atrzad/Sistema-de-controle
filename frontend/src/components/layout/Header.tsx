import { Link } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

export function Header() {
  const { usuario, logout } = useAuth();

  return (
    <header className="header">
      <span>{usuario?.nome}</span>
      <Link to="/alterar-senha" className="btn btn--ghost">
        Alterar senha
      </Link>
      <button onClick={logout} className="btn btn--ghost">
        Sair
      </button>
    </header>
  );
}
