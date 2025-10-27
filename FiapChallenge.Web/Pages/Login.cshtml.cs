using FiapChallenge.Web.Models;
using FiapChallenge.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace FiapChallenge.Web.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApiService _apiService;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public LoginViewModel LoginData { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public LoginModel(ApiService apiService, ILogger<LoginModel> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public void OnGet()
        {
            // Limpar sessão ao acessar a página de login
            HttpContext.Session.Clear();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _logger.LogInformation("========================================");
                _logger.LogInformation("=== LOGIN ATTEMPT for email: {Email} ===", LoginData.Email);
                _logger.LogInformation("========================================");

                var response = await _apiService.PostAsync("Auth/login", new
                {
                    email = LoginData.Email,
                    senha = LoginData.Senha
                });

                _logger.LogInformation("=== Login API response status: {StatusCode} ===", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("=== Raw response length: {Length} ===", content.Length);

                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(content);

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        _logger.LogInformation("=== Token received - Length: {Length} ===", loginResponse.Token.Length);
                        _logger.LogInformation("=== Token first 30 chars: {TokenStart}... ===", loginResponse.Token.Substring(0, Math.Min(30, loginResponse.Token.Length)));

                        HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                        HttpContext.Session.SetString("UserEmail", loginResponse.Email);

                        // Verify token was saved
                        var savedToken = HttpContext.Session.GetString("JwtToken");
                        _logger.LogInformation("=== Token saved to session: {IsSaved} ===", !string.IsNullOrEmpty(savedToken));
                        _logger.LogInformation("=== Saved token matches original: {Matches} ===", savedToken == loginResponse.Token);

                        if (!string.IsNullOrEmpty(savedToken))
                        {
                            _logger.LogInformation("=== Saved token first 30 chars: {TokenStart}... ===", savedToken.Substring(0, Math.Min(30, savedToken.Length)));
                        }

                        _logger.LogInformation("=== Redirecting to /Index ===");
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        _logger.LogError("=== ERROR: loginResponse is null or token is empty ===");
                        ErrorMessage = "Resposta inválida da API";
                        return Page();
                    }
                }

                _logger.LogWarning("=== Login failed with status: {StatusCode} ===", response.StatusCode);
                ErrorMessage = "E-mail ou senha inválidos";
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== Exception during login: {Message} ===", ex.Message);
                ErrorMessage = $"Erro ao conectar com a API: {ex.Message}";
                return Page();
            }
        }
    }
}
