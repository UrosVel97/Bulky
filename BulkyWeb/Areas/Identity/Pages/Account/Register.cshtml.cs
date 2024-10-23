// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Bulky.DataAccess.Reporsitory.IReporsitory;

namespace BulkyWeb.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager; //Moramo da deklarisemo i definisemo objekat klase 'RoleManager' kako bismo mogli da radimo sa ulogama
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender; //Ovaj interfejs moramo mi da implementiramo, jer on nije sam po sebi implementiran
        private readonly IUnitOfWork _unitOfWork; //Obaj interfejs ce nam trebati za vracanje objekta iz baze tipa 'Company'
        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger, 
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;//Ovaj interfejs moramo mi da implementiramo, jer on nije sam po sebi implementiran
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }


            /*Unutar klase 'InputModel' smo dodali jos jedno polje 'Role' koje nam sluzi da odredimo koju ulogu korisnik zeli da ima.
             * Takodje smo dodali listu tipa 'SelectListItem' koja ce nam sluziti da popunimo dropdown listu sa ulogama koje korisnik moze da izabere kada se registruje.*/
            public string Role { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }


            /* Dodajemo sledece propertije unutar klase 'InputModel'. Kada
             * korisnik hoce da se registruje morace da popuni sva polja koja su mapirana na ove
             * propertije. 
             * */
            [Required]
            public string Name { get; set; }
            public string? StreetAddress { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? PostalCode { get; set; }
            public string? PhoneNumber { get; set; }

            /*Sledece dve linije koda su zaduzene za selekciju Kompanije i dodeljivanje te selektovane kompanije
             * korisniku koji se prvi put registruje*/
            public int? CompanyId { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> CompanyList { get; set; }



        }


        public async Task OnGetAsync(string returnUrl = null)
        {



            /*Imamo properi 'Input' koje je tipa klase 'InputModel' i njega instaciramo tako sto cemo samo
            da instanciramo njegov properti 'RoleList' koje je tipa 'IEnumerable<SelectListItem>'.*/

            Input = new()
            {
                /*Za instanciranje liste 'RoleList', mi cemo da vratimo sve nazive uloga iz tabele
                 * 'AspNetRoles'. Necemo da koristimo objekat klase 'ApplicationDbContext', vec cemo da koristimo
                 * properti '_roleManager' koji isto ima pristup bazi, tacnije ima pristup tabeli 'AspNetRoles'. 
                 * Preko njega mi cemo da vratimo sve nazive uloga iz tabele. Zatim cemo da za svaki naziv uloge
                 * kreirati objekat klase 'SelectListItem' i naziv uloge cemo dodeliti kao vrtednost propertija
                 * 'Text' i 'Value' klase 'SelectListItem'. Na taj nacin mi smo popunili listu 'RoleList' sa objektima
                 * klase 'SelectListItem'.*/
                /* Znaci objekat '_roleManager.Roles' vraca sve uloge iz tabele 'AspNetRoles'. Zatim uzima samo nazive uloga, i dobijamo listu
                 * naziva uloga. Zatim za svaki element u listi naziva uloga, kreirace se novi objekat klase 'SelectListItem'
                 * i naziv uloge ce se dodeliti njegovim propertijima 'Text' i 'Value' kao vrednost. Na kraj imamo
                 * listu objekata klase 'SelectListItem' koju smo dedelili listi 'RoleList'.*/
                RoleList = _roleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                //Pravimo listu kompanija iz koje ce korisnik moci da selektuje jednu kompaniju da mu bude dodeljena pri registraciji
                CompanyList = _unitOfWork.Kompanija.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };



            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                /* Kada korisnik klikne na dugme za registraciju, mi moramo u ovom delu koda da
                 * naglasimo da ce taj novi slog u tabeli 'AspNetUsers' imati popunjena i ostala polja kolona koje smo naknadno dodali.
                 * Koristricemo klasu 'ApplicationUser' za kreiranje korisnika.
                 * */
                user.StreetAddress = Input.StreetAddress;
                user.City = Input.City;
                user.PostalCode = Input.PostalCode;
                user.PhoneNumber = Input.PhoneNumber;
                user.Name = Input.Name;
                user.State= Input.State;

                /*Ako je korisniku koji treba da se registruje dodeljena
                 * uloga 'Company' onda cemo i properti 'CompanyId' klase 'ApplicationUser'
                 * da mu dodelimo vrednost id-ja kompanije koje je selektovao korisnik.*/
                if(Input.Role==SD.Role_Company)
                {
                    user.CompanyId=Input.CompanyId;
                }


                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    
                    //Proverravamo da li je krajnji korisnik izabrao ulogu iz liste uloga kada se registrovao
                    if(!String.IsNullOrEmpty(Input.Role))
                    {
                        //Ako jeste onda dodeljujemo ulogu tom korisniku, i pravimo novi slog u tabli 'AspNetUserRoles'. Taj slog predstavlja vezu izmedju Korisnika i njegove uloge
                        //Mi mozemo dodeliti vise uloga korisniku, to se radi pomocu metode '_userManager.AddToRolesAsync()', metoda kao prametar prihvata listu naziva uloga.
                        await _userManager.AddToRoleAsync(user,Input.Role);
                    }
                    else
                    {
                        //Ako korisnik nije izabrao ulogu iz liste uloga onda dodeljujemo ulogu 'Customer', i pravimo novi slog u tabli 'AspNetUserRoles'. Taj slog predstavlja vezu izmedju Korisnika i njegove uloge
                        await _userManager.AddToRoleAsync(user,SD.Role_Customer);
                    }
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        if (User.IsInRole(SD.Role_Admin))
                        {
                            TempData["success"] = "Korisnik je uspesno kreiran";
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);

                        }
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        //Ova metoda nam sluzi da naglasimo da ce kolona 'Discriminator' u tabeli 'AspNetUsers' biti popunjena sa vrednoscu 'ApplicationUser'
        private ApplicationUser CreateUser()
        {
            try
            {
                //Vracamo instancuy klase 'ApplicationUser'
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
