import { Routes, Route, Navigate } from 'react-router-dom';
import { PrivateRoute } from './PrivateRoute';
import { LoginPage } from '../pages/login/LoginPage';
import { DashboardPage } from '../pages/dashboard/DashboardPage';
import { ProfessoresPage } from '../pages/professores/ProfessoresPage';
import { TurmasPage } from '../pages/turmas/TurmasPage';
import { SalasPage } from '../pages/salas/SalasPage';
import { CronogramaPage } from '../pages/cronograma/CronogramaPage';
import { FrequenciaPage } from '../pages/frequencia/FrequenciaPage';
import { TurmasAfetadasPage } from '../pages/ausencias/TurmasAfetadasPage';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      <Route element={<PrivateRoute />}>
        <Route path="/" element={<DashboardPage />} />
        <Route path="/professores" element={<ProfessoresPage />} />
        <Route path="/turmas" element={<TurmasPage />} />
        <Route path="/salas" element={<SalasPage />} />
        <Route path="/cronograma" element={<CronogramaPage />} />
        <Route path="/frequencia" element={<FrequenciaPage />} />
        <Route path="/turmas-afetadas" element={<TurmasAfetadasPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
