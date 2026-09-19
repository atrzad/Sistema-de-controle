namespace SistemaDeControle.Application.DTOs.Justificativas;

public record JustificativaResponseDto(
    int Id,
    int RegistroFrequenciaId,
    int ProfessorId,
    string ProfessorNome,
    DateOnly DataAusencia,
    string Motivo,
    DateTime DataEnvio,
    List<AnexoResponseDto> Anexos);
