using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Razor;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IFollowRepository _followRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public List<CheepDTO> Cheeps { get; set; } = new();
    public List<string> Following { get; set; } = new();

    //for header display
    public string CurrentDisplayName { get; private set; } = "";

    public int CurrentPage { get; set; } = 1;
    public bool HasMorePages { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Cheep message is required")]
    [StringLength(160, ErrorMessage = "Cheep cannot exceed 160 characters")]
    public required string Text { get; set; }

    public PublicModel(
        ICheepService cheepService,
        IFollowRepository followRepository,
        UserManager<ApplicationUser> userManager)
    {
        _cheepService = cheepService;
        _followRepository = followRepository;
        _userManager = userManager;
    }

    public async Task OnGet([FromQuery] int page = 1, [FromQuery] string? sort = null)
    {
        if (page < 1) page = 1;
        CurrentPage = page;

        // Resolve display name for header
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            CurrentDisplayName = user?.DisplayName ?? User.Identity!.Name!;
        }

        Cheeps = await _cheepService.GetCheeps(page, User.Identity?.Name);

        Cheeps = sort switch
        {
            "liked" => Cheeps.OrderByDescending(c => c.LikeCount).ToList(),
            _ => Cheeps.OrderByDescending(c => DateTime.Parse(c.Timestamp)).ToList()
        };

        HasMorePages = Cheeps.Count == 32;

        if (User.Identity?.IsAuthenticated == true)
            Following = await _followRepository.GetFollowing(User.Identity.Name!);
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            await OnGet();
            return Page();
        }

        var currentUserKey = User.Identity?.Name; // internal key (email)
        if (string.IsNullOrEmpty(currentUserKey))
            return RedirectToPage("/Account/Login");

        await _cheepService.PostCheep(new CheepDTO
        {
            Message = Text.Trim(),
            AuthorKey = currentUserKey,
            AuthorDisplayName = currentUserKey,
            Timestamp = DateTime.Now.ToString("g")
        });

        Text = string.Empty;
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostFollow(string user)
    {
        await _followRepository.Follow(User.Identity!.Name!, user);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUnfollow(string user)
    {
        await _followRepository.Unfollow(User.Identity!.Name!, user);
        return RedirectToPage();
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

    public IActionResult OnPostNext(int currentPage)
    {
        return Redirect("/?page=" + (currentPage + 1));
    }

    public IActionResult OnPostPrev(int currentPage)
    {
        int newPage = currentPage - 1;
        if (newPage < 1) newPage = 1;

        return Redirect("/?page=" + newPage);
    }
}
