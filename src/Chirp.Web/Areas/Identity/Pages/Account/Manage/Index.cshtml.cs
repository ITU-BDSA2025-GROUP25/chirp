// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IFollowRepository _followRepository;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IFollowRepository followRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _followRepository = followRepository;
        }

        public string Username { get; set; }

        public class FollowingUser
        {
            public string UserKey { get; set; }
            public string DisplayName { get; set; }
        }

        public System.Collections.Generic.List<FollowingUser> FollowingUsers { get; set; } = new();

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async System.Threading.Tasks.Task LoadAsync(ApplicationUser user)
        {
            var followerKey = User?.Identity?.Name ?? user.UserName;

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            // Display name in the UI header field (disabled input)
            Username = string.IsNullOrWhiteSpace(user.DisplayName)
                ? (user.UserName ?? "")
                : user.DisplayName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };

            FollowingUsers = new System.Collections.Generic.List<FollowingUser>();

            if (string.IsNullOrWhiteSpace(followerKey))
                return;

            var followingKeys = await _followRepository.GetFollowing(followerKey);

            if (followingKeys == null || followingKeys.Count == 0)
                return;

            // each followed user's display name
            foreach (var key in followingKeys)
            {
                if (string.IsNullOrWhiteSpace(key))
                    continue;

                var followedUser = await _userManager.FindByNameAsync(key);

                var display = followedUser != null && !string.IsNullOrWhiteSpace(followedUser.DisplayName)
                    ? followedUser.DisplayName
                    : key;

                FollowingUsers.Add(new FollowingUser
                {
                    UserKey = key,
                    DisplayName = display
                });
            }

            // Optional: sort nicely
            FollowingUsers.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, System.StringComparison.OrdinalIgnoreCase));
        }

        public async System.Threading.Tasks.Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            await LoadAsync(user);
            return Page();
        }

        public async System.Threading.Tasks.Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
