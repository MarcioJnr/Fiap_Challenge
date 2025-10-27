using FluentValidation;
using FiapChallenge.Application.DTOs;

namespace FiapChallenge.Application.Validators
{
    public class TurmaCreateValidator : AbstractValidator<TurmaCreateDto>
    {
        public TurmaCreateValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .Length(10, 250).WithMessage("Descrição deve ter entre 10 e 250 caracteres");
        }
    }

    public class TurmaUpdateValidator : AbstractValidator<TurmaUpdateDto>
    {
        public TurmaUpdateValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres");

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage("Descrição é obrigatória")
                .Length(10, 250).WithMessage("Descrição deve ter entre 10 e 250 caracteres");
        }
    }
}