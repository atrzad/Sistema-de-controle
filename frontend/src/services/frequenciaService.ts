import { api } from './api';
import type { OcorrenciaDia, RegistroFrequencia, StatusFrequencia, TurmaAfetada } from '../types/frequencia';

export const frequenciaService = {
  async getOcorrenciasDoDia(data: string): Promise<OcorrenciaDia[]> {
    const { data: resultado } = await api.get<OcorrenciaDia[]>(`/frequencia/dia/${data}`);
    return resultado;
  },

  async registrar(aulaAgendadaId: number, data: string, status: StatusFrequencia, observacao?: string): Promise<RegistroFrequencia> {
    const { data: resultado } = await api.post<RegistroFrequencia>('/frequencia', {
      aulaAgendadaId,
      data,
      status,
      observacao,
    });
    return resultado;
  },

  async atualizar(id: number, status: StatusFrequencia, observacao?: string): Promise<RegistroFrequencia> {
    const { data } = await api.put<RegistroFrequencia>(`/frequencia/${id}`, { status, observacao });
    return data;
  },

  async getTurmasAfetadas(data: string, professorId?: number): Promise<TurmaAfetada[]> {
    const { data: resultado } = await api.get<TurmaAfetada[]>('/frequencia/turmas-afetadas', {
      params: { data, professorId },
    });
    return resultado;
  },

  async list(params?: { professorId?: number; dataInicio?: string; dataFim?: string; status?: StatusFrequencia }): Promise<RegistroFrequencia[]> {
    const { data } = await api.get<RegistroFrequencia[]>('/frequencia', { params });
    return data;
  },
};
