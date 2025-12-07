using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages
{
    public class UserTimelineModel : PageModel
    {
        private readonly ICheepService _service;
        private const int PageSize = 32;

        public required List<CheepDTO> Cheeps { get; set; }
        public int CurrentPage { get; set; }
        public bool HasMorePages { get; set; }

        public UserTimelineModel(ICheepService service)
        {
            _service = service;
        }

        public async Task OnGet(string author, [FromQuery] int page = 1)
        {
            if (page < 1) page = 1;
            CurrentPage = page;

            Cheeps = await _service.GetCheepsFromAuthor(author, page);

            // If we received a full page, assume more may exist
            HasMorePages = Cheeps.Count == PageSize;
        }
    }
}