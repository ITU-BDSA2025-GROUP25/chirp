using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

 	public async Task OnGet(string author, [FromQuery] int page = 1) //defauls to page 1 if page not given
	{
    if (page < 1) page = 1; // Ensuring no 0 or negative page numbers

    Cheeps = await _service.GetCheepsFromAuthor(author, page);
    

	}
}
