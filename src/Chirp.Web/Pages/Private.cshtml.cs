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

        public int CurrentPage { get; set; } = 1;
        public bool HasMorePages { get; set; }

        private const int PageSize = 32;

        public PrivateModel(ICheepService cheepService, IFollowRepository followRepository)
        {
            _cheepService = cheepService;
            _followRepository = followRepository;
        }

        public async Task OnGet()
        {
            int page = 1;
            var pageValue = Request.Query["page"].ToString();
            if (!string.IsNullOrEmpty(pageValue) && int.TryParse(pageValue, out var parsed) && parsed > 0)
            {
                page = parsed;
            }

            CurrentPage = page;
            
            string? sort = Request.Query["sort"];

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return;

            Following = await _followRepository.GetFollowing(username);

            Cheeps = await _cheepService.GetPrivateTimeline(username, page);

            if (sort == "liked")
            {
                Cheeps = Cheeps
                    .OrderByDescending(c => c.LikeCount)
                    .ThenByDescending(c => c.Timestamp)
                    .ToList();
            }


            // If full page, assume more
            HasMorePages = Cheeps.Count == PageSize;
        }


        public async Task<IActionResult> OnPostUnfollowAsync(string user)
        {
            var currentUser = User.Identity?.Name;
            if (currentUser == null)
                return RedirectToPage("/Account/Login");

            await _followRepository.Unfollow(currentUser, user);
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
    }
}
