using System;
using System.Collections.Generic;

namespace FiapChallenge.Domain.Entities;

public partial class Usuario
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime DataCadastro { get; set; }
}
