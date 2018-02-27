using Gugleus.WebUI.Models.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Gugleus.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginVM loginVM, string returnUrl)
        {
            IActionResult result;

            if (!ModelState.IsValid)
                result = View(loginVM);
            else
            {
                _logger.LogDebug($"[{nameof(Login)}] Start for user = '{loginVM.UserName}'");

                var user = await _userManager.FindByNameAsync(loginVM.UserName);
                if (user != null)
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (signInResult.Succeeded)
                    {
                        _logger.LogDebug($"[{nameof(Login)}] Success for user = '{loginVM.UserName}'");

                        if (string.IsNullOrWhiteSpace(returnUrl))
                            result = RedirectToAction("Index", "Home");
                        else
                            result = Redirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogDebug($"[{nameof(Login)}] Failed for login = '{loginVM.UserName}' and pass = '{loginVM.Password}'");
                        ModelState.AddModelError("", "Invalid User name/password.");
                        result = View(loginVM);
                    }
                }
                else
                {
                    _logger.LogDebug($"[{nameof(Login)}] User with login = '{loginVM.UserName}' not found.");
                    ModelState.AddModelError("", "Invalid User name/password.");
                    result = View(loginVM);
                }
            }

            return result;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"[{nameof(Register)}] Start for name = '{registerVM.UserName}'");

                var user = new IdentityUser() { UserName = registerVM.UserName };
                var result = await _userManager.CreateAsync(user, registerVM.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }

                    _logger.LogDebug($"[{nameof(Register)}] Errors: '{string.Join(";", result.Errors)}'");
                }
            }
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), "Account");
        }
    }
}