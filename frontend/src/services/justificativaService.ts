import { api } from './api';
import type { Justificativa } from '../types/justificativa';

export const justificativaService = {
  async list(params?: { professorId?: number; dataInicio?: string; dataFim?: string }): Promise<Justificativa[]> {
    const { data } = await api.get<Justificativa[]>('/justificativas', { params });
    return data;
  },

  async create(registroFrequenciaId: number, motivo: string, arquivo: File): Promise<Justificativa> {
    const formData = new FormData();
    formData.append('registroFrequenciaId', String(registroFrequenciaId));
    formData.append('motivo', motivo);
    formData.append('arquivo', arquivo);

    const { data } = await api.post<Justificativa>('/justificativas', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return data;
  },

  async baixarAnexo(justificativaId: number, nomeArquivo: string): Promise<void> {
    const response = await api.get(`/justificativas/${justificativaId}/anexo`, { responseType: 'blob' });
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.download = nomeArquivo;
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
  },
};
