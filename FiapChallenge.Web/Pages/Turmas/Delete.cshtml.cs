using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Turmas
{
    public class DeleteModel : PageModel
    {
        private readonly ApiService _apiService;

        public DeleteModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public TurmaViewModel? Turma { get; set; }
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
                var turma = await _apiService.GetAsync<TurmaViewModel>($"Turmas/{id}", token);
                if (turma == null)
                {
                    return NotFound();
                }
                Turma = turma;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar turma: {ex.Message}";
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
                await _apiService.DeleteAsync($"Turmas/{id}", token);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao excluir turma: {ex.Message}";
                var turma = await _apiService.GetAsync<TurmaViewModel>($"Turmas/{id}", token);
                Turma = turma;
                return Page();
            }
        }
    }
}
