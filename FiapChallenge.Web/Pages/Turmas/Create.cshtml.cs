using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Turmas
{
    public class CreateModel : PageModel
    {
        private readonly ApiService _apiService;

        public CreateModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public TurmaViewModel Turma { get; set; } = new();

        public string? ErrorMessage { get; set; }

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
                await _apiService.PostAsync("Turmas", Turma, token);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao cadastrar turma: {ex.Message}";
                return Page();
            }
        }
    }
}
