using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

[Authorize] // Only authenticated users can access this page
public class AboutMeModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICheepService _cheepService;
    private readonly IAuthorRepository _authorRepository;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<CheepDTO> UserCheeps { get; set; } = new();
    public int TotalCheeps { get; set; }

    public AboutMeModel(
        UserManager<ApplicationUser> userManager,
        ICheepService cheepService,
        IAuthorRepository authorRepository,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _cheepService = cheepService;
        _authorRepository = authorRepository;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // Get user information
        UserName = user.UserName ?? string.Empty;
        Email = user.Email ?? string.Empty;

        // Get all cheeps by this user (without pagination)
        UserCheeps = await _cheepService.GetAllCheepsFromAuthor(UserName);
        TotalCheeps = UserCheeps.Count;

        return Page();
    }

    public async Task<IActionResult> OnPostForgetMeAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            // Get the author
            var author = await _authorRepository.GetAuthorByName(user.UserName!);
            if (author != null)
            {
                // Delete or anonymize the author and their cheeps
                await _authorRepository.DeleteAuthor(author.AuthorId);
            }

            // Delete the identity user
            await _userManager.DeleteAsync(user);

            // Sign out the user
            await _signInManager.SignOutAsync();

            // Redirect to the public timeline (root page)
            return Redirect("/");
        }
        catch (Exception ex)
        {
            // Log the error (in a real app, use proper logging)
            ModelState.AddModelError(string.Empty, "An error occurred while deleting your account.");
            return Page();
        }
    }
}
