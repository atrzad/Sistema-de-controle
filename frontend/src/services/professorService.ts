import { api } from './api';
import type { Professor, ProfessorInput } from '../types/professor';
import type { Turma } from '../types/turma';

export const professorService = {
  async list(params?: { nome?: string; ativo?: boolean }): Promise<Professor[]> {
    const { data } = await api.get<Professor[]>('/professores', { params });
    return data;
  },

  async getById(id: number): Promise<Professor> {
    const { data } = await api.get<Professor>(`/professores/${id}`);
    return data;
  },

  async create(input: ProfessorInput): Promise<Professor> {
    const { data } = await api.post<Professor>('/professores', input);
    return data;
  },

  async update(id: number, input: ProfessorInput): Promise<Professor> {
    const { data } = await api.put<Professor>(`/professores/${id}`, input);
    return data;
  },

  async inativar(id: number): Promise<void> {
    await api.delete(`/professores/${id}`);
  },

  async getTurmas(id: number): Promise<Turma[]> {
    const { data } = await api.get<Turma[]>(`/professores/${id}/turmas`);
    return data;
  },
};
