using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Matriculas
{
    public class DeleteModel : PageModel
    {
        private readonly ApiService _apiService;

        public DeleteModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public MatriculaViewModel? Matricula { get; set; }
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
                var matricula = await _apiService.GetAsync<MatriculaViewModel>($"Matriculas/{id}", token);
                if (matricula == null)
                {
                    return NotFound();
                }
                Matricula = matricula;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar matrícula: {ex.Message}";
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
                await _apiService.DeleteAsync($"Matriculas/{id}", token);
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao excluir matrícula: {ex.Message}";
                var matricula = await _apiService.GetAsync<MatriculaViewModel>($"Matriculas/{id}", token);
                Matricula = matricula;
                return Page();
            }
        }
    }
}
