import { NavLink } from 'react-router-dom';

const links = [
  { to: '/', label: 'Dashboard' },
  { to: '/professores', label: 'Professores' },
  { to: '/turmas', label: 'Turmas' },
  { to: '/salas', label: 'Salas' },
  { to: '/cronograma', label: 'Cronograma' },
  { to: '/frequencia', label: 'Frequência do dia' },
  { to: '/turmas-afetadas', label: 'Turmas afetadas' },
];

export function Sidebar() {
  return (
    <nav className="sidebar">
      <div className="sidebar__logo">Sistema de Controle</div>
      <ul>
        {links.map((link) => (
          <li key={link.to}>
            <NavLink to={link.to} end={link.to === '/'} className={({ isActive }) => (isActive ? 'active' : '')}>
              {link.label}
            </NavLink>
          </li>
        ))}
      </ul>
    </nav>
  );
}
