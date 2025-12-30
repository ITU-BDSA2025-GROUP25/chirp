#nullable disable
// This page locally overrides the scaffolded ASP.NET Identity
// DeletePersonalData page to add Chirp domain cleanup and anonymization.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class PersonalDataModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}