using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Golfklubb_Centar_Webbshop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Golfklubb_Centar_Webbshop.Areas.Identity.Pages.Account.Manage
{
    public class UserDetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _environment;


        public UserDetailsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "FullName")]
            [StringLength(100)]
            public string? FirstName { get; set; }

            [Required(ErrorMessage = "Användarnamn är obligatoriskt.")]
            [Display(Name = "UserName")]
            [StringLength(50)]
            public string UserName { get; set; }

            [Phone(ErrorMessage = "Ange ett giltigt telefonnummer.")]
            [Display(Name = "PhoneNumber")]
            public string PhoneNumber { get; set; }

            [Display(Name = "EmailAddress")]
            [StringLength(100)]
            public string? EmailAdress { get; set; }

            public string? ExistingProfileImage { get; set; }

            [Display(Name = "ProfileImage")]
            public IFormFile? ProfileImage { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            Input = new InputModel
            {
                FirstName = user.FullName,
                UserName = user.UserName,
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                EmailAdress = user.Email,
                ExistingProfileImage = user.ProfileImageUrl
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kunde inte ladda användaren med ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Kunde inte ladda användaren med ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Uppdatera namn
            user.FullName = Input.FirstName;

            // Uppdatera användarnamn om det ändrats
            if (user.UserName != Input.UserName)
            {
                var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var error in setUserNameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    await LoadAsync(user);
                    return Page();
                }
            }

            // Uppdatera telefonnummer om det ändrats
            var currentPhoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != currentPhoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    foreach (var error in setPhoneResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    await LoadAsync(user);
                    return Page();
                }
            }

            //uppdatera emailadress om det ändrats
            var currentEmailAdress = await _userManager.GetEmailAsync(user);
            if(Input.EmailAdress != currentEmailAdress)
            {
                var setEmailAdress = await _userManager.SetEmailAsync(user, Input.EmailAdress);
                if(!setEmailAdress.Succeeded)
                {
                    foreach(var error in setEmailAdress.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    await LoadAsync(user);
                    return Page();
                }
            }

            //uppdatera profilbild om det ändrats
            if (Input.ProfileImage != null && Input.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "profiles");

                // Skapa mappen om den inte finns
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Skapa unikt filnamn
                var uniqueFileName = $"{Guid.NewGuid()}_{Input.ProfileImage.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Spara filen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfileImage.CopyToAsync(fileStream);
                }

                // Ta bort gammal profilbild om den finns
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, user.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Spara sökvägen i databasen
                user.ProfileImageUrl = $"/images/profiles/{uniqueFileName}";
            }



            // Spara övriga fält
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await LoadAsync(user);
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Dina uppgifter har uppdaterats.";
            return RedirectToPage();
        }
    }
}