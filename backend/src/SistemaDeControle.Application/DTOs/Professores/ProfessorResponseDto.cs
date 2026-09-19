namespace SistemaDeControle.Application.DTOs.Professores;

public record ProfessorResponseDto(
    int Id,
    string Nome,
    string Email,
    string? Telefone,
    string Matricula,
    string? Disciplina,
    bool Ativo,
    DateTime CreatedAt);
