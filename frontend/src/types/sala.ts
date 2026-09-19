export interface Sala {
  id: number;
  nome: string;
  capacidade?: number | null;
  ativo: boolean;
}

export interface SalaInput {
  nome: string;
  capacidade?: number | null;
  ativo?: boolean;
}
