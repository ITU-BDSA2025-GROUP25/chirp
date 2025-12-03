using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PrivateModel : PageModel
{
    private readonly ICheepService _cheepService;

    public required List<CheepDTO> Cheeps { get; set; }

    public PrivateModel(ICheepService cheepService)
    {
        _cheepService = cheepService;
    }

    public async Task<IActionResult> OnGet(int page = 1)
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToPage("/Account/Login");

        var username = User.Identity.Name!;
        Cheeps = await _cheepService.GetPrivateTimeline(username, page);

        return Page();
    }
}