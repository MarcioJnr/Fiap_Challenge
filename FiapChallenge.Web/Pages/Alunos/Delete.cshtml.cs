using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Alunos
{
    public class DeleteModel : PageModel
    {
        private readonly ApiService _apiService;

        public DeleteModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public AlunoViewModel? Aluno { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                var aluno = await _apiService.GetAsync<AlunoViewModel>($"Alunos/{id}", token);
                if (aluno == null)
                {
                    return NotFound();
                }
                Aluno = aluno;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar aluno: {ex.Message}";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                await _apiService.DeleteAsync($"Alunos/{id}", token);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao excluir aluno: {ex.Message}";
                var aluno = await _apiService.GetAsync<AlunoViewModel>($"Alunos/{id}", token);
                Aluno = aluno;
                return Page();
            }
        }
    }
}
