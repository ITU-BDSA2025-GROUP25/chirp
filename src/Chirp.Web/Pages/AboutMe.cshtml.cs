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
        Console.WriteLine("[DEBUG] ForgetMe button clicked!");
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            Console.WriteLine("[DEBUG] User is null, redirecting to login");
            return RedirectToPage("/Login");
        }

        Console.WriteLine($"[DEBUG] Deleting user: {user.UserName}");

        try
        {
            // Get the author
            var author = await _authorRepository.GetAuthorByName(user.UserName!);
            if (author != null)
            {
                Console.WriteLine($"[DEBUG] Found author with ID: {author.AuthorId}, deleting...");
                // Delete the author and their cheeps
                await _authorRepository.DeleteAuthor(author.AuthorId);
                Console.WriteLine($"[DEBUG] Author deleted successfully");
            }
            else
            {
                Console.WriteLine($"[DEBUG] No author found for username: {user.UserName}");
            }

            // Delete the identity user
            Console.WriteLine($"[DEBUG] Deleting Identity user...");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                Console.WriteLine($"[DEBUG] Failed to delete user. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                ModelState.AddModelError(string.Empty, "Failed to delete user account.");
                return Page();
            }
            Console.WriteLine($"[DEBUG] Identity user deleted successfully");

            // Sign out the user
            Console.WriteLine($"[DEBUG] Signing out user...");
            await _signInManager.SignOutAsync();
            Console.WriteLine($"[DEBUG] User signed out successfully");

            // Redirect to the home page
            Console.WriteLine($"[DEBUG] Redirecting to home page...");
            return LocalRedirect("~/");
        }
        catch (Exception ex)
        {
            // Log the error (in a real app, use proper logging)
            Console.WriteLine($"[ERROR] Exception deleting account: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
            ModelState.AddModelError(string.Empty, "An error occurred while deleting your account.");
            return Page();
        }
    }
}
