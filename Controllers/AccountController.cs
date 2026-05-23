using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using digital_library.Data;
using digital_library.Models;

namespace digital_library.Controllers;

public class AccountController : Controller
{
    private const string SessionUserId = "UserId";
    private const string SessionUsername = "Username";
    private const string SessionOAuthState = "OAuthState";

    private const string GoogleAuthEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string GoogleTokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string GoogleUserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

    private readonly digital_libraryContext _context;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(
        digital_libraryContext context,
        IConfiguration config,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    // GET: Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32(SessionUserId) is not null)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var input = model.UsernameOrEmail.Trim();
        var user = _context.User.FirstOrDefault(u => u.Username == input || u.Email == input);

        if (user is null || user.PasswordHash is null || !PasswordHasher.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid username/email or password.");
            return View(model);
        }

        SignIn(user);
        TempData["StatusMessage"] = $"Welcome back, {user.Username}!";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // GET: Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32(SessionUserId) is not null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    // POST: Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var email = model.Email.Trim();

        if (_context.User.Any(u => u.Username == username))
        {
            ModelState.AddModelError(nameof(model.Username), "That username is already taken.");
            return View(model);
        }

        if (_context.User.Any(u => u.Email == email))
        {
            ModelState.AddModelError(nameof(model.Email), "That email is already registered.");
            return View(model);
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.Hash(model.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.User.Add(user);
        _context.SaveChanges();

        SignIn(user);
        TempData["StatusMessage"] = $"Welcome, {user.Username}! Your account has been created.";
        return RedirectToAction("Index", "Home");
    }

    // GET: Account/GoogleLogin - kicks off the Google OAuth authorization-code flow.
    [HttpGet]
    public IActionResult GoogleLogin()
    {
        var clientId = _config["GoogleAuth:ClientId"];
        var redirectUri = _config["GoogleAuth:RedirectUri"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
        {
            TempData["StatusMessage"] = "Google sign-in is not configured.";
            return RedirectToAction("Login");
        }

        // A random state value, stored in session, guards against CSRF on the callback.
        var state = Guid.NewGuid().ToString("N");
        HttpContext.Session.SetString(SessionOAuthState, state);

        var authUrl = $"{GoogleAuthEndpoint}" +
            $"?client_id={Uri.EscapeDataString(clientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
            "&response_type=code" +
            $"&scope={Uri.EscapeDataString("openid email profile")}" +
            $"&state={Uri.EscapeDataString(state)}" +
            "&prompt=select_account";

        return Redirect(authUrl);
    }

    // GET: Account/callback - Google redirects here with an authorization code.
    [HttpGet]
    public async Task<IActionResult> Callback(string? code, string? state, string? error)
    {
        if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(code))
        {
            TempData["StatusMessage"] = "Google sign-in was cancelled.";
            return RedirectToAction("Login");
        }

        var expectedState = HttpContext.Session.GetString(SessionOAuthState);
        HttpContext.Session.Remove(SessionOAuthState);
        if (string.IsNullOrEmpty(state) || state != expectedState)
        {
            TempData["StatusMessage"] = "Google sign-in failed (invalid state). Please try again.";
            return RedirectToAction("Login");
        }

        var accessToken = await ExchangeCodeForAccessTokenAsync(code);
        if (accessToken is null)
        {
            TempData["StatusMessage"] = "Could not complete Google sign-in. Please try again.";
            return RedirectToAction("Login");
        }

        var googleUser = await GetGoogleUserAsync(accessToken);
        if (googleUser is null)
        {
            TempData["StatusMessage"] = "Could not read your Google profile. Please try again.";
            return RedirectToAction("Login");
        }

        // Find an existing account by Google id first, then fall back to the email.
        var user = _context.User.FirstOrDefault(u => u.GoogleId == googleUser.Id)
                   ?? _context.User.FirstOrDefault(u => u.Email == googleUser.Email);

        if (user is null)
        {
            user = new User
            {
                Username = GenerateUniqueUsername(googleUser.Name),
                Email = googleUser.Email,
                GoogleId = googleUser.Id,
                PasswordHash = null,
                CreatedAt = DateTime.UtcNow
            };
            _context.User.Add(user);
            _context.SaveChanges();
        }
        else if (user.GoogleId is null)
        {
            // Link an existing email/password account to this Google identity.
            user.GoogleId = googleUser.Id;
            _context.SaveChanges();
        }

        SignIn(user);
        TempData["StatusMessage"] = $"Welcome, {user.Username}!";
        return RedirectToAction("Index", "Home");
    }

    // POST: Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["StatusMessage"] = "You have been logged out.";
        return RedirectToAction("Login");
    }

    private void SignIn(User user)
    {
        HttpContext.Session.SetInt32(SessionUserId, user.Id);
        HttpContext.Session.SetString(SessionUsername, user.Username);
    }

    private async Task<string?> ExchangeCodeForAccessTokenAsync(string code)
    {
        var clientId = _config["GoogleAuth:ClientId"];
        var clientSecret = _config["GoogleAuth:ClientSecret"];
        var redirectUri = _config["GoogleAuth:RedirectUri"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
        {
            return null;
        }

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret,
            ["redirect_uri"] = redirectUri,
            ["grant_type"] = "authorization_code"
        });

        var client = _httpClientFactory.CreateClient();
        using var response = await client.PostAsync(GoogleTokenEndpoint, form);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("access_token", out var token)
            ? token.GetString()
            : null;
    }

    private async Task<GoogleUserInfo?> GetGoogleUserAsync(string accessToken)
    {
        var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, GoogleUserInfoEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var id = root.TryGetProperty("sub", out var subEl) ? subEl.GetString() : null;
        var email = root.TryGetProperty("email", out var emailEl) ? emailEl.GetString() : null;
        var name = root.TryGetProperty("name", out var nameEl) ? nameEl.GetString() : null;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
        {
            return null;
        }

        // Reject an account whose email Google explicitly reports as unverified.
        if (root.TryGetProperty("email_verified", out var verifiedEl)
            && verifiedEl.ValueKind == JsonValueKind.False)
        {
            return null;
        }

        return new GoogleUserInfo(id, email, string.IsNullOrWhiteSpace(name) ? email : name);
    }

    private string GenerateUniqueUsername(string preferred)
    {
        var baseName = string.IsNullOrWhiteSpace(preferred) ? "user" : preferred.Trim();
        var candidate = baseName;
        var suffix = 1;
        while (_context.User.Any(u => u.Username == candidate))
        {
            candidate = $"{baseName}{suffix++}";
        }

        return candidate;
    }

    private sealed record GoogleUserInfo(string Id, string Email, string Name);
}
