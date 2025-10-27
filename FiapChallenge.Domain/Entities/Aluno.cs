using System;
using System.Collections.Generic;

namespace FiapChallenge.Domain.Entities;

public partial class Aluno
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    public string Cpf { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public DateTime DataCadastro { get; set; }

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
