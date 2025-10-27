using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FiapChallenge.Web.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Limpar sess�o
            HttpContext.Session.Clear();

            // Redirecionar para login
            return RedirectToPage("/Login");
        }
    }
}
