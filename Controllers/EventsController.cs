
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly RaceDayApiService _api;

        public EventsController(RaceDayApiService api)
        {
            _api = api;
        }

        // GET: /Events
        [HttpGet]
        public async Task<IActionResult> Index(
            string? search, string? type)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            ViewBag.Search = search;
            ViewBag.Type = type ?? "All";

            try
            {
                using var response = await _api.GetAsync("api/events");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Unable to load events.";
                    return View(new List<EventViewModel>());
                }

                var events = await response.Content
                    .ReadFromJsonAsync<List<EventViewModel>>()
                    ?? new List<EventViewModel>();

                var filtered = events
                    .Where(e => e.EventDate.Date >= DateTime.Today)
                    .Where(e => e.Status != "Cancelled" &&
                                e.Status != "Completed")
                    .AsEnumerable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    filtered = filtered.Where(e =>
                        e.EventName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase)
                        ||
                        e.Location.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrWhiteSpace(type) && type != "All")
                {
                    filtered = filtered.Where(e =>
                        e.EventType.Equals(
                            type,
                            StringComparison.OrdinalIgnoreCase));
                }

                return View(filtered
                    .OrderBy(e => e.EventDate)
                    .ToList());
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Cannot connect to the RaceDay API.";
            }
            catch (TaskCanceledException)
            {
                ViewBag.Error = "The API request timed out.";
            }
            catch (JsonException)
            {
                ViewBag.Error = "Invalid event information returned.";
            }

            return View(new List<EventViewModel>());
        }

        // GET: /Events/Details/1
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            if (id <= 0)
                return NotFound();

            try
            {
                using var response = await _api.GetAsync(
                    $"api/events/{id}");

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return NotFound("Event not found.");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] =
                        "Unable to load event details.";

                    return RedirectToAction("Index");
                }

                var raceEvent = await response.Content
                    .ReadFromJsonAsync<EventDetailsViewModel>();

                if (raceEvent == null)
                    return NotFound();

                return View(raceEvent);
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
            catch (JsonException)
            {
                TempData["ErrorMessage"] =
                    "The API returned invalid event details.";
            }

            return RedirectToAction("Index");
        }

        // POST: /Events/Enroll/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(
            int id, int categoryId)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Index", "Home");

            if (id <= 0)
                return NotFound();

            if (categoryId <= 0)
            {
                TempData["ErrorMessage"] =
                    "Please select a category.";

                return RedirectToAction("Details", new { id });
            }

            try
            {
                var request = new
                {
                    categoryID = categoryId
                };

                using var response = await _api.PostAsync(
                    $"api/events/{id}/enrollments",
                    request);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] =
                        "You have successfully enrolled in the event!";

                    return RedirectToAction("Details", new { id });
                }

                string errorMessage =
                    "Your enrolment could not be completed.";

                try
                {
                    using var document = JsonDocument.Parse(
                        await response.Content.ReadAsStringAsync());

                    if (document.RootElement.TryGetProperty(
                        "message", out var message))
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

            return RedirectToAction("Details", new { id });
        }
    }
}
