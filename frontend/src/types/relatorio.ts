export interface RelatorioFrequencia {
  professorId: number;
  professorNome: string;
  dataInicio: string;
  dataFim: string;
  totalAulas: number;
  totalPresencas: number;
  totalAusencias: number;
  totalAusenciasJustificadas: number;
  percentualPresenca: number;
}
