import { api } from './api';
import type { Sala, SalaInput } from '../types/sala';

export const salaService = {
  async list(params?: { ativo?: boolean }): Promise<Sala[]> {
    const { data } = await api.get<Sala[]>('/salas', { params });
    return data;
  },

  async getById(id: number): Promise<Sala> {
    const { data } = await api.get<Sala>(`/salas/${id}`);
    return data;
  },

  async create(input: SalaInput): Promise<Sala> {
    const { data } = await api.post<Sala>('/salas', input);
    return data;
  },

  async update(id: number, input: SalaInput): Promise<Sala> {
    const { data } = await api.put<Sala>(`/salas/${id}`, input);
    return data;
  },

  async inativar(id: number): Promise<void> {
    await api.delete(`/salas/${id}`);
  },
};
