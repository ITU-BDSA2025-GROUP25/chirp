using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IFollowRepository _followRepository;

    public required List<CheepDTO> Cheeps { get; set; }
    public List<string> Following { get; set; } = new();

    [BindProperty]
    [Required(ErrorMessage = "Cheep message is required")]
    [StringLength(160, ErrorMessage = "Cheep cannot exceed 160 characters")]
    public required string Text { get; set; }

    public PublicModel(ICheepService cheepService, IFollowRepository followRepository)
    {
        _cheepService = cheepService;
        _followRepository = followRepository;
    }

    public async Task OnGet([FromQuery] int page = 1)
    {
        if (page < 1) page = 1;

        Cheeps = await _cheepService.GetCheeps(page);

        if (User.Identity!.IsAuthenticated)
            Following = await _followRepository.GetFollowing(User.Identity.Name!);
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await OnGet();
            return Page();
        }

        var currentUser = User.Identity?.Name;
        if (string.IsNullOrEmpty(currentUser))
            return RedirectToPage("/Account/Login");

        var cheep = new CheepDTO
        {
            Message = Text.Trim(),
            AuthorName = currentUser,
            Timestamp = DateTime.Now.ToString("g")
        };

        await _cheepService.PostCheep(cheep);

        Text = string.Empty;
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostFollow(string user)
    {
        await _followRepository.Follow(User.Identity!.Name!, user);
        return Redirect("/");
    }

    public async Task<IActionResult> OnPostUnfollow(string user)
    {
        await _followRepository.Unfollow(User.Identity!.Name!, user);
        return Redirect("/");
    }

    public async Task<IActionResult> OnPostLike(int cheepId)
    {
        await _cheepService.LikeCheep(cheepId, User.Identity!.Name!);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUnlike(int cheepId)
    {
        await _cheepService.UnlikeCheep(cheepId, User.Identity!.Name!);
        return RedirectToPage();
    }
}
