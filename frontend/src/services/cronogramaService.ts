import { api } from './api';
import type { AulaAgendada, AulaAgendadaInput, GradeDia } from '../types/cronograma';

export const cronogramaService = {
  async list(params?: { professorId?: number; turmaId?: number; salaId?: number; diaSemana?: string }): Promise<AulaAgendada[]> {
    const { data } = await api.get<AulaAgendada[]>('/cronograma', { params });
    return data;
  },

  async getGradeSemanal(): Promise<GradeDia[]> {
    const { data } = await api.get<GradeDia[]>('/cronograma/grade-semanal');
    return data;
  },

  async getByProfessor(professorId: number): Promise<AulaAgendada[]> {
    const { data } = await api.get<AulaAgendada[]>(`/cronograma/professor/${professorId}`);
    return data;
  },

  async getByTurma(turmaId: number): Promise<AulaAgendada[]> {
    const { data } = await api.get<AulaAgendada[]>(`/cronograma/turma/${turmaId}`);
    return data;
  },

  async create(input: AulaAgendadaInput): Promise<AulaAgendada> {
    const { data } = await api.post<AulaAgendada>('/cronograma', input);
    return data;
  },

  async update(id: number, input: AulaAgendadaInput): Promise<AulaAgendada> {
    const { data } = await api.put<AulaAgendada>(`/cronograma/${id}`, input);
    return data;
  },

  async inativar(id: number): Promise<void> {
    await api.delete(`/cronograma/${id}`);
  },
};
