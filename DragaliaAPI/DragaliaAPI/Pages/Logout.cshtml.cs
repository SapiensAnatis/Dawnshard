using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DragaliaAPI.Pages;

[AllowAnonymous]
public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        await this.HttpContext.SignOutAsync();

        return this.LocalRedirect("~/");
    }
}
