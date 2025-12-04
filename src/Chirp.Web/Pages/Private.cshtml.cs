using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages
{
    public class PrivateModel : PageModel
    {
        private readonly ICheepService _cheepService;
        private readonly IFollowRepository _followRepository;

        public List<CheepDTO> Cheeps { get; set; } = new();
        public List<string> Following { get; set; } = new();

        public PrivateModel(ICheepService cheepService, IFollowRepository followRepository)
        {
            _cheepService = cheepService;
            _followRepository = followRepository;
        }

        public async Task OnGet()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return;

            Following = await _followRepository.GetFollowing(username);
            Cheeps = await _cheepService.GetPrivateTimeline(username);
        }

        public async Task<IActionResult> OnPostUnfollowAsync(string user)
        {
            var currentUser = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUser)) return RedirectToPage("/Account/Login");

            await _followRepository.Unfollow(currentUser, user);
            return RedirectToPage();
        }

    }
}