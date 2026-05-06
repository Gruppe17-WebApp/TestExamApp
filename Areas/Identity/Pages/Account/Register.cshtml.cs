#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace TestExamApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Navn er påkrevd.")]
            [Display(Name = "Navn")]
            public string Name { get; set; }

            [Required(ErrorMessage = "E-post er påkrevd.")]
            [EmailAddress(ErrorMessage = "Ugyldig e-post.")]
            [Display(Name = "E-post")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Passord er påkrevd.")]
            [StringLength(100, ErrorMessage = "{0} må være minst {2} og maks {1} tegn.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Passord")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Bekreft passord er påkrevd.")]
            [DataType(DataType.Password)]
            [Display(Name = "Bekreft passord")]
            [Compare("Password", ErrorMessage = "Passordene samsvarer ikke.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return Page();

            //  SJEKK OM E-POST ALLEREDE FINNES
            var existingUser = await _userManager.FindByEmailAsync(Input.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "En bruker med denne e-posten finnes allerede.");
                return Page();
            }

            var user = new IdentityUser();

            // login fortsatt med e-post
            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    var message = error.Description;

                    //  Norsk oversettelse av vanlige Identity-feil
                    if (message.Contains("Passwords must be at least"))
                        message = "Passordet må være minst 6 tegn.";

                    else if (message.Contains("uppercase"))
                        message = "Passordet må inneholde minst én stor bokstav.";

                    else if (message.Contains("lowercase"))
                        message = "Passordet må inneholde minst én liten bokstav.";

                    else if (message.Contains("digit"))
                        message = "Passordet må inneholde minst ett tall.";

                    else if (message.Contains("non alphanumeric"))
                        message = "Passordet må inneholde minst ett spesialtegn.";

                    else if (message.Contains("already taken"))
                        message = "En bruker med denne e-posten finnes allerede.";

                    ModelState.AddModelError(string.Empty, message);
                }

                return Page();
            }

            _logger.LogInformation("Bruker opprettet.");

            //  lagre navn som claim
            var claimResult = await _userManager.AddClaimAsync(
                user,
                new Claim("FullName", Input.Name)
            );

            if (!claimResult.Succeeded)
            {
                ModelState.AddModelError("", "Kunne ikke lagre navn.");
                return Page();
            }

            // ingn auto login
            TempData["SuccessMessage"] = "Registrering vellykket! Du kan nå logge inn.";

            return RedirectToPage("./Login");
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
                throw new NotSupportedException("Krever brukerlagring med e-post.");

            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}