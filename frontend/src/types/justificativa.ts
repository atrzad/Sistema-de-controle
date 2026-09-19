export interface Anexo {
  id: number;
  nomeArquivoOriginal: string;
  tipoConteudo: string;
  tamanhoBytes: number;
  createdAt: string;
}

export interface Justificativa {
  id: number;
  registroFrequenciaId: number;
  professorId: number;
  professorNome: string;
  dataAusencia: string;
  motivo: string;
  dataEnvio: string;
  anexos: Anexo[];
}
