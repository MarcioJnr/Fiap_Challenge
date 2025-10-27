using System;
using System.Collections.Generic;

namespace FiapChallenge.Domain.Entities;

public partial class VwTurmasComAluno
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public DateTime DataCadastro { get; set; }

    public int? QuantidadeAlunos { get; set; }
}
