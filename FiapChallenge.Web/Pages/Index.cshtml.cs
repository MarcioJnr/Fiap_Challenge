using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages
{
    public class IndexModel : PageModel
    {
        public string UserEmail { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            UserEmail = HttpContext.Session.GetString("UserEmail") ?? "Usuário";
            return Page();
        }
    }
}
