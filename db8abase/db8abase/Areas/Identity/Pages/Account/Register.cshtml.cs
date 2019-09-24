using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using db8abase.Models;
using db8abase.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace db8abase.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public List<SelectListItem> UserRoles { get; private set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Super Admin")]
            public bool isSuperAdmin { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required]
            public string Role { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            UserRoles = new List<SelectListItem>()
            {
                new SelectListItem {Value = "TournamentDirector", Text = "TournamentDirector"},
                new SelectListItem {Value = "Coach", Text = "Coach"},
                new SelectListItem {Value = "Debater", Text = "Debater"},
                new SelectListItem {Value = "Judge", Text = "Judge"},
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Role = Input.Role,
                    Name = Input.Name,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber,
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.TournamentDirector))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.TournamentDirector));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Coach))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Coach));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Debater))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Debater));
                    }
                    if (!await _roleManager.RoleExistsAsync(StaticDetails.Judge))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(StaticDetails.Judge));
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    if (user.Role == "TournamentDirector")
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.TournamentDirector);
                        var role = await _userManager.GetRolesAsync(user);
                        return RedirectToAction("Create", "TournamentDirectors", new { id = user.Id });
                    }
                    if (user.Role == "Coach")
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.Coach);
                        var role = await _userManager.GetRolesAsync(user);
                        return RedirectToAction("Create", "Coaches", new { id = user.Id });
                    }
                    if (user.Role == "Debater")
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.Debater);
                        var role = await _userManager.GetRolesAsync(user);
                        return RedirectToAction("Create", "Debaters", new { id = user.Id });
                    }
                    if (user.Role == "Judge")
                    {
                        await _userManager.AddToRoleAsync(user, StaticDetails.Judge);
                        var role = await _userManager.GetRolesAsync(user);
                        return RedirectToAction("Create", "Judges", new { id = user.Id });
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
