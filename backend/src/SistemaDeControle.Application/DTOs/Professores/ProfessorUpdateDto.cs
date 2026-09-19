namespace SistemaDeControle.Application.DTOs.Professores;

public record ProfessorUpdateDto(string Nome, string Email, string? Telefone, string Matricula, string? Disciplina, bool Ativo);
