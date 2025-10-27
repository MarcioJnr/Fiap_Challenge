using System.ComponentModel.DataAnnotations;

namespace FiapChallenge.Web.Models
{
    public class MatriculaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Aluno é obrigatório")]
        public int AlunoId { get; set; }

        [Required(ErrorMessage = "Turma é obrigatória")]
        public int TurmaId { get; set; }

        public string? NomeAluno { get; set; }

        public string? NomeTurma { get; set; }

        public DateTime DataMatricula { get; set; }
    }
}
