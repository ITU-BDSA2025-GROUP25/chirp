#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ChirpDbContext _db;
        private readonly ILogger<DeletePersonalDataModel> _logger;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ChirpDbContext db,
            ILogger<DeletePersonalDataModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool RequirePassword { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Password is required.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            RequirePassword = await _userManager.HasPasswordAsync(user);

            // Initialize model for clean binding/validation
            Input = new InputModel();

            return Page();
        }

        // Handles the deletion request when the form is submitted
        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            RequirePassword = await _userManager.HasPasswordAsync(user);

            if (RequirePassword)
            {
                // Trigger validation required password)
                if (!ModelState.IsValid)
                    return Page();

                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            // Key used in Chirp domain tables (follows/likes/authors).
            var userKey = User.Identity?.Name ?? user.UserName;
            if (string.IsNullOrWhiteSpace(userKey))
                return BadRequest("Could not determine the current user's key.");

            // ---- Chirp domain cleanup/anonymization ----

            // Remove follow relationships
            var follows = await _db.Follows
                .Where(f => f.Follower == userKey || f.Followee == userKey)
                .ToListAsync();
            if (follows.Count > 0)
                _db.Follows.RemoveRange(follows);

            // Remove likes by this user
            var likes = await _db.Likes
                .Where(l => l.Username == userKey)
                .ToListAsync();
            if (likes.Count > 0)
                _db.Likes.RemoveRange(likes);

            // Anonymize author so cheeps no longer display personal identity
            var anonEmail = $"deleted-{Guid.NewGuid():N}@anon.invalid";
            var author = await _db.Authors.FirstOrDefaultAsync(a => a.Email == userKey);
            if (author != null)
            {
                //when deletion happened set name to "deleted user and email to anonEmail to scramble identity
                author.Name = "Deleted user";
                author.Email = anonEmail;
            }

            await _db.SaveChangesAsync();
            
            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var login in logins)
            {
                await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }

            // ---- Identity delete logout ----
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError(string.Empty, e.Description);

                return Page();
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User '{UserId}' deleted their account via Forget me.", user.Id);

            return Redirect("~/");
        }
    }
}
