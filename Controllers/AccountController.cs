
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly RaceDayApiService _api;

        public AccountController(RaceDayApiService api)
        {
            _api = api;
        }

        // ============================================
        // GET: /Account/Register
        // Display the registration page.
        // ============================================

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // ============================================
        // POST: /Account/Register
        // Register through the existing RaceDay API.
        // ============================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new
            {
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password,
                model.Role
            };

            try
            {
                using var response = await _api.PostAsync(
                    "api/auth/register",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Account created successfully. Please log in.";

                    return RedirectToAction("Login");
                }

                string error = await ReadApiMessageAsync(
                    response,
                    "Registration failed. Please check your details.");

                ModelState.AddModelError(
                    string.Empty,
                    error);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cannot connect to the RaceDay API. " +
                    "Make sure the API is running.");
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API request timed out. Please try again.");
            }

            return View(model);
        }

        // ============================================
        // GET: /Account/Login
        // Display the login page.
        // ============================================

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("Role") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        // ============================================
        // POST: /Account/Login
        // Authenticate through the Part 2 API.
        // ============================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Clear old local login information before
            // starting a new login session.
            HttpContext.Session.Clear();
            _api.ClearApiSession();

            try
            {
                using var response = await _api.PostAsync(
                    "api/auth/login",
                    new
                    {
                        model.Email,
                        model.Password
                    });

                if (!response.IsSuccessStatusCode)
                {
                    string error = await ReadApiMessageAsync(
                        response,
                        "Incorrect email or password.");

                    ModelState.AddModelError(
                        string.Empty,
                        error);

                    return View(model);
                }

                using var document = JsonDocument.Parse(
                    await response.Content.ReadAsStringAsync());

                var root = document.RootElement;

                if (!root.TryGetProperty(
                        "userID",
                        out var userIDProperty) ||
                    !userIDProperty.TryGetInt32(
                        out int userID) ||
                    !root.TryGetProperty(
                        "role",
                        out var roleProperty) ||
                    roleProperty.ValueKind != JsonValueKind.String)
                {
                    _api.ClearApiSession();

                    ModelState.AddModelError(
                        string.Empty,
                        "The API returned an invalid login response.");

                    return View(model);
                }

                string role = roleProperty.GetString()
                    ?? string.Empty;

                if (userID <= 0 ||
                    (role != "Organiser" &&
                     role != "Participant"))
                {
                    _api.ClearApiSession();

                    ModelState.AddModelError(
                        string.Empty,
                        "The account role could not be verified.");

                    return View(model);
                }

                string firstName = root.TryGetProperty(
                    "firstName",
                    out var firstNameProperty)
                    ? firstNameProperty.GetString() ?? ""
                    : "";

                // Save authenticated user details in
                // the MVC application's server-side session.
                HttpContext.Session.SetInt32(
                    "UserID",
                    userID);

                HttpContext.Session.SetString(
                    "Role",
                    role);

                HttpContext.Session.SetString(
                    "FirstName",
                    firstName);

                HttpContext.Session.SetString(
                    "Email",
                    model.Email);

                TempData["SuccessMessage"] =
                    "Welcome to RaceDay, " + firstName + "!";

                // Later we will route users to their
                // role-specific dashboards.
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Cannot connect to the RaceDay API. " +
                    "Make sure the API is running.");
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The API request timed out. Please try again.");
            }
            catch (JsonException)
            {
                _api.ClearApiSession();

                ModelState.AddModelError(
                    string.Empty,
                    "The API returned an invalid response.");
            }

            return View(model);
        }

        // ============================================
        // POST: /Account/Logout
        // Clear both API and MVC login sessions.
        // ============================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                using var response = await _api.PostAsync(
                    "api/auth/logout",
                    new { });
            }
            catch (HttpRequestException)
            {
                // The local session will still be cleared.
            }
            catch (TaskCanceledException)
            {
                // The local session will still be cleared.
            }

            _api.ClearApiSession();
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }

        // ============================================
        // Read an error message returned by the API.
        // ============================================

        private static async Task<string> ReadApiMessageAsync(
            HttpResponseMessage response,
            string fallback)
        {
            try
            {
                using var document = JsonDocument.Parse(
                    await response.Content.ReadAsStringAsync());

                if (document.RootElement.TryGetProperty(
                        "message",
                        out var messageProperty) &&
                    messageProperty.ValueKind ==
                        JsonValueKind.String)
                {
                    return messageProperty.GetString()
                        ?? fallback;
                }
            }
            catch (JsonException)
            {
                // Use the fallback message.
            }

            return fallback;
        }
    }
}
