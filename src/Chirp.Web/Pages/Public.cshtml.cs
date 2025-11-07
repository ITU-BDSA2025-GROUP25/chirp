using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
	private readonly ICheepService _service;
	private readonly ICheepRepository _repository;
	public required List<CheepDTO> Cheeps { get; set; }
    
	[BindProperty] // This is crucial for form binding
	public string Text { get; set; }

	public PublicModel(ICheepService service, ICheepRepository repository)
	{
		_service = service;
		_repository = repository;
	}

	public async Task OnGet([FromQuery] int page = 1)
	{
		if (page < 1) page = 1;
		Cheeps = await _service.GetCheeps(page);
	}

	public async Task<IActionResult> OnPost()
	{
		if (string.IsNullOrEmpty(Text))
		{
			// Optional: Add error handling
			ModelState.AddModelError(nameof(Text), "Cheep message cannot be empty");
			await OnGet(); // Reload cheeps to display the page properly
			return Page();
		}

		// Get the current user
		var currentUser = User.Identity.Name;
		if (string.IsNullOrEmpty(currentUser))
		{
			// This shouldn't happen since the form is only shown to signed-in users
			return RedirectToPage("/Account/Login");
		}

		// Create the cheep - you'll need to handle the Author properly
		var cheep = new CheepDTO
		{
			Message = Text,
			Author = new Author
			{
				Name = currentUser,
				Email = currentUser
				
			}, // Set the author from logged-in user
			Timestamp = DateTime.Now.ToString("g")
		};

		await _repository.CreateCheep(cheep);
        
		// Clear the text after successful post
		Text = string.Empty;
        
		// Redirect to prevent form resubmission
		return RedirectToPage();
	}
}
