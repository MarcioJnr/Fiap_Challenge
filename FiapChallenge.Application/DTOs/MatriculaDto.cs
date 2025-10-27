namespace FiapChallenge.Application.DTOs
{
    public class MatriculaCreateDto
    {
        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
    }

    public class MatriculaResponseDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public string NomeAluno { get; set; } = string.Empty;
        public int TurmaId { get; set; }
        public string NomeTurma { get; set; } = string.Empty;
        public DateTime DataMatricula { get; set; }
    }
}