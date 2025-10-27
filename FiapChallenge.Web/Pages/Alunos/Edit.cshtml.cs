using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FiapChallenge.Web.Pages.Alunos
{
    public class EditModel : PageModel
    {
        private readonly ApiService _apiService;

        public EditModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [BindProperty]
        public AlunoViewModel Aluno { get; set; } = new();

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
                await _apiService.PutAsync($"Alunos/{Aluno.Id}", Aluno, token);
                return RedirectToPage("Index");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                ErrorMessage = "Erro de validação: Verifique os dados informados.";
                ModelState.AddModelError(string.Empty, "Dados inválidos. Verifique os campos e tente novamente.");
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao atualizar aluno: {ex.Message}";
                return Page();
            }
        }
    }
}
