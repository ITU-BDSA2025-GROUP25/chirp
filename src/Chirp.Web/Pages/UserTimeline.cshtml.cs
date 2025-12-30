using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Razor;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages
{
    public class UserTimelineModel : PageModel
    {
        private readonly ICheepService _service;
        private readonly UserManager<ApplicationUser> _userManager;

        private const int PageSize = 32;

        public List<CheepDTO> Cheeps { get; set; } = new();
        public int CurrentPage { get; set; }
        public bool HasMorePages { get; set; }

        //display name in header
        public string TimelineDisplayName { get; private set; } = "";

        public UserTimelineModel(ICheepService service, UserManager<ApplicationUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        public async Task OnGet(string author, [FromQuery] int page = 1)
        {
            if (page < 1) page = 1;
            CurrentPage = page;

            // author is the route value, which is the EMAIL KEY
            var authorKey = author;

            //convert key to display name for header
            var user = await _userManager.FindByEmailAsync(authorKey);
            TimelineDisplayName = user?.DisplayName ?? authorKey;

            Cheeps = await _service.GetCheepsFromAuthor(authorKey, page);

            HasMorePages = Cheeps.Count == PageSize;
        }
    }
}