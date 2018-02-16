using Gugleus.WebUI.Models;
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
            // TODO: prepare SQL with Login tables
            // TODO: returnUrl
            if (!ModelState.IsValid)
                return View(loginVM);

            _logger.LogDebug($"[{nameof(Login)}] Start for name = '{loginVM.UserName}'");

            var user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user != null)
            {
                var result = await
                    _signInManager.PasswordSignInAsync
                    (user, loginVM.Password, false, false);

                if (result.Succeeded)
                {
                    _logger.LogDebug($"[{nameof(Login)}] Success for name = '{loginVM.UserName}'");
                    return RedirectToAction("Index", "Home");
                }
            }

            _logger.LogDebug($"[{nameof(Login)}] Failed for login = '{loginVM.UserName}' and pass = '{loginVM.Password}'");
            ModelState.AddModelError("", "User name/password not found");
            return View(loginVM);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                _logger.LogDebug($"[{nameof(Register)}] Start for name = '{loginVM.UserName}'");

                var user = new IdentityUser() { UserName = loginVM.UserName };
                var result = await _userManager.CreateAsync(user, loginVM.Password);

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
            return View(loginVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login), "Account");
        }
    }
}