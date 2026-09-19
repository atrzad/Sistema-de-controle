import { api } from './api';
import type { RelatorioFrequencia } from '../types/relatorio';
import type { TurmaAfetada } from '../types/frequencia';

export const relatorioService = {
  async getSemanal(professorId: number, dataReferencia: string): Promise<RelatorioFrequencia> {
    const { data } = await api.get<RelatorioFrequencia>('/relatorios/frequencia/semanal', {
      params: { professorId, dataReferencia },
    });
    return data;
  },

  async getMensal(professorId: number, mes: number, ano: number): Promise<RelatorioFrequencia> {
    const { data } = await api.get<RelatorioFrequencia>('/relatorios/frequencia/mensal', {
      params: { professorId, mes, ano },
    });
    return data;
  },

  async getConsolidado(dataInicio: string, dataFim: string): Promise<RelatorioFrequencia[]> {
    const { data } = await api.get<RelatorioFrequencia[]>('/relatorios/frequencia/consolidado', {
      params: { dataInicio, dataFim },
    });
    return data;
  },

  async getTurmasAfetadasHistorico(dataInicio: string, dataFim: string): Promise<TurmaAfetada[]> {
    const { data } = await api.get<TurmaAfetada[]>('/relatorios/turmas-afetadas', {
      params: { dataInicio, dataFim },
    });
    return data;
  },
};
