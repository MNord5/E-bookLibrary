using E_bookLibrary.Data;
using E_bookLibrary.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace E_bookLibrary.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        public AccountController(AppDbContext context)
        {
                _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(AppUser user)
        {
            if (ModelState.IsValid)
            {
                var userExist = _context.Users.FirstOrDefault(x => x.UserName == user.UserName && x.Email == user.Email);
                if (userExist == null)
                {
                    ModelState.AddModelError("UserName", "No user found, check if name and email is correct");
                    return View(user);
                }
                if (!VerifyPassword(user.Password, userExist.PasswordHash, userExist.PasswordSalt))
                {
                    ModelState.AddModelError("Password", "Incorrect password");
                    return View(user);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userExist.Id)),
                    new Claim(ClaimTypes.Name, userExist.UserName),
                    new Claim("Email", userExist.Email),
                    new Claim(ClaimTypes.Role, "User"),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties()
                    {
                        IsPersistent = true
                    });

                return RedirectToAction("Index", "Library");
            }

            return View(user);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AppUser request)
        {
            if (ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == request.Email))
                {
                    return View(request);
                }
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var user = new UserModel
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    VerificationToken = CreateRandomToken()
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private string CreateRandomToken()
        {
            Guid obj = Guid.NewGuid();
            return obj.ToString();
        }

    }
}
