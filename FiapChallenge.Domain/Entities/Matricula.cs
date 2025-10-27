using FiapChallenge.Domain.Entities;
using System;
using System.Collections.Generic;

namespace FiapChallenge.Domain.Entities;

public partial class Matricula
{
    public int Id { get; set; }

    public int AlunoId { get; set; }

    public int TurmaId { get; set; }

    public DateTime DataMatricula { get; set; }

    public virtual Aluno Aluno { get; set; } = null!;

    public virtual Turma Turma { get; set; } = null!;
}
