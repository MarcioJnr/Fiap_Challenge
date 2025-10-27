using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Limpar sessão
            HttpContext.Session.Clear();

            // Redirecionar para login
            return RedirectToPage("/Login");
        }
    }
}
