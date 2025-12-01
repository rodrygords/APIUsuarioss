namespace APIUsuarios.Application.DTOs;

public record UsuarioCreateDto(
    string Nome,
    string Email,
    string Senha,
    DateTime DataNascimento,
    string? Telefone
);

public record UsuarioReadDto(
    int Id,
    string Nome,
    string Email,
    DateTime DataNascimento,
    string? Telefone,
    bool Ativo,
    DateTime DataCriacao
);

public record UsuarioUpdateDto(
    string Nome,
    string Email,
    DateTime DataNascimento,
    string? Telefone,
    bool Ativo
);