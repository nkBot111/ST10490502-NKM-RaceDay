
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly RaceDayApiService _api;

        public EnrollmentsController(RaceDayApiService api)
        {
            _api = api;
        }

        // GET: /Enrollments
        // Display the current Participant's enrolments.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Participant")
            {
                return RedirectToAction("Index", "Home");
            }

            try
            {
                using var response = await _api.GetAsync(
                    "api/enrollments/me");

                if (response.StatusCode ==
                    HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();

                    return RedirectToAction("Login", "Account");
                }

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Unable to load your enrolments.";

                    return View(new List<EnrollmentViewModel>());
                }

                var enrollments = await response.Content
                    .ReadFromJsonAsync<List<EnrollmentViewModel>>()
                    ?? new List<EnrollmentViewModel>();

                return View(enrollments
                    .OrderByDescending(e => e.EnrollmentDate)
                    .ToList());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error =
                    "Cannot connect to the RaceDay API. " +
                    "Please make sure the API is running.";
            }
            catch (TaskCanceledException)
            {
                ViewBag.Error =
                    "The request timed out. Please try again.";
            }
            catch (JsonException)
            {
                ViewBag.Error =
                    "The API returned invalid enrolment information.";
            }

            return View(new List<EnrollmentViewModel>());
        }

        // POST: /Enrollments/Cancel/1
        // Cancel an eligible Participant enrolment.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            int? userID = HttpContext.Session.GetInt32("UserID");
            string? role = HttpContext.Session.GetString("Role");

            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Participant")
            {
                return RedirectToAction("Index", "Home");
            }

            if (id <= 0)
            {
                TempData["ErrorMessage"] =
                    "Invalid enrolment selected.";

                return RedirectToAction("Index");
            }

            try
            {
                using var response = await _api.DeleteAsync(
                    $"api/enrollments/{id}");

                if (response.StatusCode ==
                    HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();

                    return RedirectToAction("Login", "Account");
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "Your enrolment has been cancelled successfully.";

                    return RedirectToAction("Index");
                }

                string errorMessage =
                    "Your enrolment could not be cancelled.";

                try
                {
                    using var document = JsonDocument.Parse(
                        await response.Content.ReadAsStringAsync());

                    if (document.RootElement.TryGetProperty(
                        "message", out var message) &&
                        message.ValueKind == JsonValueKind.String)
                    {
                        errorMessage =
                            message.GetString() ?? errorMessage;
                    }
                }
                catch (JsonException)
                {
                    // Keep the default error message.
                }

                TempData["ErrorMessage"] = errorMessage;
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Cannot connect to the RaceDay API.";
            }
            catch (TaskCanceledException)
            {
                TempData["ErrorMessage"] =
                    "The API request timed out.";
            }

            return RedirectToAction("Index");
        }
    }
}
