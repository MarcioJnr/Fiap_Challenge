using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace FiapChallenge.Web.Pages.Alunos
{
    public class IndexModel : PageModel
    {
        private readonly ApiService _apiService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApiService apiService, ILogger<IndexModel> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public List<AlunoViewModel> Alunos { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int pageNumber = 1)
        {
            _logger.LogInformation("========================================");
            _logger.LogInformation("=== ALUNOS INDEX OnGetAsync START ===");
            _logger.LogInformation("========================================");

            var token = HttpContext.Session.GetString("JwtToken");
            _logger.LogInformation("Token retrieved from session");
            _logger.LogInformation("Token is null: {IsNull}", token == null);
            _logger.LogInformation("Token is empty: {IsEmpty}", string.IsNullOrEmpty(token));

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("Token length: {Length}", token.Length);
                _logger.LogInformation("Token first 30 chars: {TokenStart}...", token.Substring(0, Math.Min(30, token.Length)));
            }

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("=== NO TOKEN FOUND - Redirecting to login ===");
                return RedirectToPage("/Login");
            }

            try
            {
                CurrentPage = pageNumber;
                _logger.LogInformation("=== Calling API to fetch alunos with token (page {Page}, size {Size}) ===", CurrentPage, PageSize);
                var response = await _apiService.GetAsync<PaginatedResponse<AlunoViewModel>>($"Alunos?pageNumber={CurrentPage}&pageSize={PageSize}", token);

                if (response != null && response.Items != null)
                {
                    Alunos = response.Items;
                    TotalCount = response.TotalCount;
                    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
                    _logger.LogInformation("=== Successfully loaded {Count} alunos (page {Page} of {Total}) ===", Alunos.Count, CurrentPage, TotalPages);
                }
                else
                {
                    _logger.LogWarning("=== API returned null response or null Items ===");
                    ErrorMessage = "Nenhum aluno encontrado.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== ERROR loading alunos: {Message} ===", ex.Message);
                ErrorMessage = $"Erro ao carregar alunos: {ex.Message}";
            }

            _logger.LogInformation("========================================");
            _logger.LogInformation("=== ALUNOS INDEX OnGetAsync END ===");
            _logger.LogInformation("========================================");
            return Page();
        }
    }
}
