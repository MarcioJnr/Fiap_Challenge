namespace FiapChallenge.Web.Models
{
    public class TurmaComAlunosViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public List<AlunoMatriculadoViewModel> Alunos { get; set; } = new();
    }

    public class AlunoMatriculadoViewModel
    {
        public int MatriculaId { get; set; }
        public int AlunoId { get; set; }
        public string NomeAluno { get; set; } = string.Empty;
        public DateTime DataMatricula { get; set; }
    }
}
