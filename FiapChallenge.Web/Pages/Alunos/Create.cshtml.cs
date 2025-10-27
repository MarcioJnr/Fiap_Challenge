using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages.Alunos
{
    public class CreateModel : PageModel
    {
        private readonly ApiService _apiService;

        public CreateModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public AlunoViewModel Aluno { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
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
                return Page();
            }

            try
            {
                await _apiService.PostAsync("Alunos", Aluno, token);
                return RedirectToPage("Index");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ErrorMessage = "Erro de validação: Verifique se a senha atende aos requisitos (8+ caracteres, maiúsculas, minúsculas, números e símbolos) e se o CPF é válido.";
                ModelState.AddModelError(string.Empty, "Dados inválidos. Verifique os campos e tente novamente.");
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao cadastrar aluno: {ex.Message}";
                return Page();
            }
        }
    }
}
