import { api } from './api';
import type { Turma, TurmaInput } from '../types/turma';
import type { Professor } from '../types/professor';

export const turmaService = {
  async list(params?: { anoLetivo?: number; turno?: string; ativo?: boolean }): Promise<Turma[]> {
    const { data } = await api.get<Turma[]>('/turmas', { params });
    return data;
  },

  async getById(id: number): Promise<Turma> {
    const { data } = await api.get<Turma>(`/turmas/${id}`);
    return data;
  },

  async create(input: TurmaInput): Promise<Turma> {
    const { data } = await api.post<Turma>('/turmas', input);
    return data;
  },

  async update(id: number, input: TurmaInput): Promise<Turma> {
    const { data } = await api.put<Turma>(`/turmas/${id}`, input);
    return data;
  },

  async inativar(id: number): Promise<void> {
    await api.delete(`/turmas/${id}`);
  },

  async getProfessores(id: number): Promise<Professor[]> {
    const { data } = await api.get<Professor[]>(`/turmas/${id}/professores`);
    return data;
  },
};
