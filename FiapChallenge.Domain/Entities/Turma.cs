using System;
using System.Collections.Generic;

namespace FiapChallenge.Domain.Entities;
public partial class Turma
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public DateTime DataCadastro { get; set; }

    public virtual ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
