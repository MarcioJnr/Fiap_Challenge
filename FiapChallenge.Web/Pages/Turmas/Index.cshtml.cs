using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace FiapChallenge.Web.Pages.Turmas
{
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;

        public IndexModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        public List<TurmaViewModel> Turmas { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            try
            {
                CurrentPage = pageNumber;
                var response = await _apiService.GetAsync<PaginatedResponse<TurmaViewModel>>($"Turmas?pageNumber={CurrentPage}&pageSize={PageSize}", token);
                if (response != null && response.Items != null)
                {
                    Turmas = response.Items;
                    TotalCount = response.TotalCount;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro ao carregar turmas: {ex.Message}";
            }

            return Page();
        }
    }
}
