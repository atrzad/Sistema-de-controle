import { useAuth } from '../../context/AuthContext';

export function Header() {
  const { usuario, logout } = useAuth();

  return (
    <header className="header">
      <span>{usuario?.nome}</span>
      <button onClick={logout} className="btn btn--ghost">
        Sair
      </button>
    </header>
  );
}
