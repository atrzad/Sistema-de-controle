export type DiaSemana = 'Domingo' | 'Segunda' | 'Terca' | 'Quarta' | 'Quinta' | 'Sexta' | 'Sabado';

export const DIAS_SEMANA: DiaSemana[] = ['Segunda', 'Terca', 'Quarta', 'Quinta', 'Sexta'];

export const DIA_SEMANA_LABEL: Record<DiaSemana, string> = {
  Domingo: 'Domingo',
  Segunda: 'Segunda-feira',
  Terca: 'Terça-feira',
  Quarta: 'Quarta-feira',
  Quinta: 'Quinta-feira',
  Sexta: 'Sexta-feira',
  Sabado: 'Sábado',
};

export interface AulaAgendada {
  id: number;
  professorId: number;
  professorNome: string;
  turmaId: number;
  turmaNome: string;
  salaId: number;
  salaNome: string;
  diaSemana: DiaSemana;
  horaInicio: string;
  horaFim: string;
  disciplina?: string | null;
  vigenteDesde?: string | null;
  vigenteAte?: string | null;
  ativo: boolean;
}

export interface AulaAgendadaInput {
  professorId: number;
  turmaId: number;
  salaId: number;
  diaSemana: DiaSemana;
  horaInicio: string;
  horaFim: string;
  disciplina?: string;
  vigenteDesde?: string | null;
  vigenteAte?: string | null;
  ativo?: boolean;
}

export interface GradeDia {
  diaSemana: DiaSemana;
  aulas: AulaAgendada[];
}
