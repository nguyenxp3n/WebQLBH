using System.Security.Claims;
using DbQLBH_65132292.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DbQLBH_65132292.Controllers
{
    [EnableRateLimiting("AuthPolicy")]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher<string> _passwordHasher;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IConfiguration configuration,
            IPasswordHasher<string> passwordHasher,
            ILogger<AccountController> logger)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToLocal(returnUrl);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var configuredUsername = _configuration["AdminAuth:Username"];
            var configuredPasswordHash = _configuration["AdminAuth:PasswordHash"];

            if (string.IsNullOrWhiteSpace(configuredUsername) || string.IsNullOrWhiteSpace(configuredPasswordHash))
            {
                _logger.LogError("Admin authentication credentials are not configured in system settings.");
                ModelState.AddModelError(string.Empty, "Hệ thống chưa được cấu hình tài khoản quản trị.");
                return View(model);
            }

            var normalizedUsername = model.Username?.Trim() ?? string.Empty;
            if (!string.Equals(normalizedUsername, configuredUsername, StringComparison.Ordinal))
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                return View(model);
            }

            var verifyResult = _passwordHasher.VerifyHashedPassword(
                normalizedUsername,
                configuredPasswordHash,
                model.Password ?? string.Empty);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, normalizedUsername),
                new(ClaimTypes.Name, normalizedUsername),
                new(ClaimTypes.Role, "Admin"),
                new("display_name", "Nguyen Admin")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(model.RememberMe ? 12 : 8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
