export type Turno = 'Manha' | 'Tarde' | 'Noite';

export interface Turma {
  id: number;
  nome: string;
  turno: Turno;
  anoLetivo: number;
  ativo: boolean;
}

export interface TurmaInput {
  nome: string;
  turno: Turno;
  anoLetivo: number;
  ativo?: boolean;
}
