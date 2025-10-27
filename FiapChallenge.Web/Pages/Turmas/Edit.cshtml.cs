using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Turmas
{
    public class EditModel : PageModel
    {
        private readonly ApiService _apiService;

        public EditModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public TurmaViewModel Turma { get; set; } = new();

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                await _apiService.PutAsync($"Turmas/{Turma.Id}", Turma, token);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao atualizar turma: {ex.Message}";
                return Page();
            }
        }
    }
}
