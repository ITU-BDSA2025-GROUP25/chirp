using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PrivateModel : PageModel
{
    private readonly ICheepRepository _repository;

    public required List<CheepDTO> Cheeps { get; set; }

    public PrivateModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> OnGet(int page = 1)
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToPage("/Account/Login");

        var username = User.Identity.Name!;
        Cheeps = await _repository.GetPrivateTimeline(username, page);

        return Page();
    }
}
