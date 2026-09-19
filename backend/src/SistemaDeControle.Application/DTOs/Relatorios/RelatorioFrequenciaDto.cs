namespace SistemaDeControle.Application.DTOs.Relatorios;

public record RelatorioFrequenciaDto(
    int ProfessorId,
    string ProfessorNome,
    DateOnly DataInicio,
    DateOnly DataFim,
    int TotalAulas,
    int TotalPresencas,
    int TotalAusencias,
    int TotalAusenciasJustificadas,
    double PercentualPresenca);
