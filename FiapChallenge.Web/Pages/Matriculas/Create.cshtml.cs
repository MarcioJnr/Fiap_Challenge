using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Matriculas
{
    public class CreateModel : PageModel
    {
        private readonly ApiService _apiService;

        public CreateModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public MatriculaViewModel Matricula { get; set; } = new();

        public List<AlunoViewModel> Alunos { get; set; } = new();
        public List<TurmaViewModel> Turmas { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? turmaId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                var alunosResponse = await _apiService.GetAsync<PaginatedResponse<AlunoViewModel>>("Alunos", token);
                var turmasResponse = await _apiService.GetAsync<PaginatedResponse<TurmaViewModel>>("Turmas", token);

                Alunos = alunosResponse?.Items ?? new();
                Turmas = turmasResponse?.Items ?? new();

                if (turmaId.HasValue)
                {
                    Matricula.TurmaId = turmaId.Value;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar dados: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                var alunosResponse = await _apiService.GetAsync<PaginatedResponse<AlunoViewModel>>("Alunos", token);
                var turmasResponse = await _apiService.GetAsync<PaginatedResponse<TurmaViewModel>>("Turmas", token);

                Alunos = alunosResponse?.Items ?? new();
                Turmas = turmasResponse?.Items ?? new();
                return Page();
            }

            try
            {
                await _apiService.PostAsync("Matriculas", Matricula, token);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao cadastrar matrícula: {ex.Message}";
                var alunosResponse = await _apiService.GetAsync<PaginatedResponse<AlunoViewModel>>("Alunos", token);
                var turmasResponse = await _apiService.GetAsync<PaginatedResponse<TurmaViewModel>>("Turmas", token);

                Alunos = alunosResponse?.Items ?? new();
                Turmas = turmasResponse?.Items ?? new();
                return Page();
            }
        }
    }
}
