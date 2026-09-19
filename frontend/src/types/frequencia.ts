import type { DiaSemana } from './cronograma';

export type StatusFrequencia = 'Presente' | 'Ausente' | 'AusenciaJustificada';

export const STATUS_LABEL: Record<StatusFrequencia, string> = {
  Presente: 'Presente',
  Ausente: 'Ausente',
  AusenciaJustificada: 'Ausência justificada',
};

export interface RegistroFrequencia {
  id: number;
  aulaAgendadaId: number;
  professorId: number;
  professorNome: string;
  turmaNome: string;
  salaNome: string;
  diaSemana: DiaSemana;
  horaInicio: string;
  horaFim: string;
  data: string;
  status: StatusFrequencia;
  observacao?: string | null;
  registradoPorUsuarioId: number;
  temJustificativa: boolean;
}

export interface OcorrenciaDia {
  aulaAgendadaId: number;
  professorId: number;
  professorNome: string;
  turmaId: number;
  turmaNome: string;
  salaId: number;
  salaNome: string;
  horaInicio: string;
  horaFim: string;
  disciplina?: string | null;
  registroFrequenciaId?: number | null;
  status?: StatusFrequencia | null;
}

export interface TurmaAfetada {
  data: string;
  professorId: number;
  professorNome: string;
  turmaId: number;
  turmaNome: string;
  salaId: number;
  salaNome: string;
  horaInicio: string;
  horaFim: string;
  status: StatusFrequencia;
}
