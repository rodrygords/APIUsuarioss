using FluentValidation;
using APIUsuarios.Application.DTOs;
using System.Text.RegularExpressions;

namespace APIUsuarios.Application.Validators;

public class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    public UsuarioUpdateDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email deve ter formato válido");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória")
            .Must(BeAtLeast18YearsOld).WithMessage("Usuário deve ter pelo menos 18 anos");

        RuleFor(x => x.Telefone)
            .Must(BeValidPhoneNumber).When(x => !string.IsNullOrEmpty(x.Telefone))
            .WithMessage("Telefone deve estar no formato (XX) XXXXX-XXXX");
    }

    private bool BeAtLeast18YearsOld(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
        return idade >= 18;
    }

    private bool BeValidPhoneNumber(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone)) return true;
        var regex = new Regex(@"^\(\d{2}\)\s\d{5}-\d{4}$");
        return regex.IsMatch(telefone);
    }
}