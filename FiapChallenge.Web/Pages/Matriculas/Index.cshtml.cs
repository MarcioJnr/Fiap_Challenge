using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Matriculas
{
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;

        public IndexModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public List<TurmaComAlunosViewModel> TurmasComAlunos { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                var turmasResponse = await _apiService.GetAsync<PaginatedResponse<TurmaViewModel>>("Turmas?pageNumber=1&pageSize=100", token);

                if (turmasResponse != null && turmasResponse.Items != null)
                {
                    foreach (var turma in turmasResponse.Items)
                    {
                        var turmaComAlunos = new TurmaComAlunosViewModel
                        {
                            Id = turma.Id,
                            Nome = turma.Nome,
                            Descricao = turma.Descricao
                        };

                        try
                        {
                            var matriculas = await _apiService.GetAsync<List<MatriculaResponseDto>>($"Matriculas/turma/{turma.Id}", token);

                            if (matriculas != null)
                            {
                                turmaComAlunos.Alunos = matriculas.Select(m => new AlunoMatriculadoViewModel
                                {
                                    MatriculaId = m.Id,
                                    AlunoId = m.AlunoId,
                                    NomeAluno = m.NomeAluno ?? "Nome não disponível",
                                    DataMatricula = m.DataMatricula
                                }).ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao buscar alunos da turma {turma.Id}: {ex.Message}");
                        }

                        TurmasComAlunos.Add(turmaComAlunos);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar matrículas: {ex.Message}";
            }

            return Page();
        }
    }

    public class MatriculaResponseDto
    {
        public int Id { get; set; }
        public int AlunoId { get; set; }
        public string? NomeAluno { get; set; }
        public int TurmaId { get; set; }
        public string? NomeTurma { get; set; }
        public DateTime DataMatricula { get; set; }
    }
}
