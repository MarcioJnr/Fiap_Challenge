using FluentValidation;
using FiapChallenge.Application.DTOs;
using System.Text.RegularExpressions;

namespace FiapChallenge.Application.Validators
{
    public class AlunoCreateValidator : AbstractValidator<AlunoCreateDto>
    {
        public AlunoCreateValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("Data de nascimento é obrigatória")
                .Must(BeAValidDate).WithMessage("Data de nascimento inválida");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Must(BeAValidCpf).WithMessage("CPF inválido");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório")
                .EmailAddress().WithMessage("E-mail inválido")
                .MaximumLength(100).WithMessage("E-mail deve ter no máximo 100 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
                .Must(BeAStrongPassword).WithMessage("Senha deve conter letras maiúsculas, minúsculas, números e símbolos especiais");
        }

        private bool BeAValidDate(DateTime date)
        {
            var minDate = DateTime.Now.AddYears(-120);
            var maxDate = DateTime.Now;
            return date >= minDate && date <= maxDate;
        }

        private bool BeAValidCpf(string cpf)
        {
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            if (cpf.Length != 11)
                return false;

            if (cpf.Distinct().Count() == 1)
                return false;

            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        private bool BeAStrongPassword(string password)
        {
            var hasUpperCase = new Regex(@"[A-Z]");
            var hasLowerCase = new Regex(@"[a-z]");
            var hasDigit = new Regex(@"[0-9]");
            var hasSpecialChar = new Regex(@"[!@#$%^&*(),.?""':{}|<>]");

            return hasUpperCase.IsMatch(password) &&
                   hasLowerCase.IsMatch(password) &&
                   hasDigit.IsMatch(password) &&
                   hasSpecialChar.IsMatch(password);
        }
    }

    public class AlunoUpdateValidator : AbstractValidator<AlunoUpdateDto>
    {
        public AlunoUpdateValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .Length(3, 100).WithMessage("Nome deve ter entre 3 e 100 caracteres");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("Data de nascimento é obrigatória")
                .Must(BeAValidDate).WithMessage("Data de nascimento inválida");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-mail é obrigatório")
                .EmailAddress().WithMessage("E-mail inválido")
                .MaximumLength(100).WithMessage("E-mail deve ter no máximo 100 caracteres");
        }

        private bool BeAValidDate(DateTime date)
        {
            var minDate = DateTime.Now.AddYears(-126);
            var maxDate = DateTime.Now;
            return date >= minDate && date <= maxDate;
        }
    }
}