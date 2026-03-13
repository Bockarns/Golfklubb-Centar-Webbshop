// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Pages.Account.Manage
{
    public class UsernameModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsernameModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public string CurrentUserName { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Nytt användarnamn")]
            public string NewUsername { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CurrentUserName = user.UserName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if(!ModelState.IsValid)
            {
                CurrentUserName = user.UserName;
                return Page();
            }

            var currentUserName = user.UserName;

            if (Input.NewUsername == currentUserName)
            {
                StatusMessage = "Användarnamnet är oförändrat";
                return RedirectToPage();
            }

            var existingUser = await _userManager.FindByNameAsync(Input.NewUsername);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Användarnamnet upptaget!");
                CurrentUserName = currentUserName;
                return Page();
            }

            var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.NewUsername);
            if (!setUserNameResult.Succeeded)
            {
                StatusMessage = "Oförväntat fel vid sparande av nytt användarnamn!";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Ditt användarnamn är bytt!";

            return RedirectToPage();
        }


    }
}
