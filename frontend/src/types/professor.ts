export interface Professor {
  id: number;
  nome: string;
  email: string;
  telefone?: string | null;
  matricula: string;
  disciplina?: string | null;
  ativo: boolean;
  createdAt: string;
}

export interface ProfessorInput {
  nome: string;
  email: string;
  telefone?: string;
  matricula: string;
  disciplina?: string;
  ativo?: boolean;
}
