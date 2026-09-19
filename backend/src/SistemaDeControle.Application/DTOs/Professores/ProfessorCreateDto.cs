namespace SistemaDeControle.Application.DTOs.Professores;

public record ProfessorCreateDto(string Nome, string Email, string? Telefone, string Matricula, string? Disciplina);
