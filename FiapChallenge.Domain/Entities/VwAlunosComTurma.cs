using System;
using System.Collections.Generic;

namespace FiapChallenge.Domain.Entities;

public partial class VwAlunosComTurma
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Cpf { get; set; } = null!;

    public DateTime DataNascimento { get; set; }

    public DateTime DataCadastro { get; set; }

    public int? QuantidadeTurmas { get; set; }
}
