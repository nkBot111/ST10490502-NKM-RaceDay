using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Web.Controllers;

public class OrganiserController : Controller
{
    private readonly RaceDayApiService _api;
    public OrganiserController(RaceDayApiService api) => _api = api;
    private bool IsOrganiser => HttpContext.Session.GetString("Role") == "Organiser" && HttpContext.Session.GetInt32("UserID") is > 0;
    private IActionResult NotOrganiser() => RedirectToAction("Login", "Account");

    private static async Task<string> ErrorAsync(HttpResponseMessage response)
    {
        try
        {
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            if (doc.RootElement.TryGetProperty("message", out var message))
                return message.GetString() ?? $"API returned {(int)response.StatusCode}.";
        }
        catch (JsonException) { }
        return $"Request failed (HTTP {(int)response.StatusCode}).";
    }
    private static string ConnectionError(Exception ex) => ex is HttpRequestException
        ? "Cannot reach the API. Start it with bash scripts/run-api.sh."
        : "The API request could not be completed.";

    private async Task<EventViewModel?> OwnedEventAsync(int id)
    {
        using var response = await _api.GetAsync($"api/events/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var evt = await response.Content.ReadFromJsonAsync<EventViewModel>();
        return evt?.OrganiserID == HttpContext.Session.GetInt32("UserID") ? evt : null;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsOrganiser) return NotOrganiser();
        try
        {
            using var response = await _api.GetAsync("api/events");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ErrorAsync(response);
                return View(new List<EventViewModel>());
            }
            var events = await response.Content.ReadFromJsonAsync<List<EventViewModel>>() ?? new();
            return View(events.Where(e => e.OrganiserID == HttpContext.Session.GetInt32("UserID"))
                .OrderByDescending(e => e.EventDate).ToList());
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["ErrorMessage"] = ConnectionError(ex);
            return View(new List<EventViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create() => IsOrganiser ? View(new EventFormViewModel()) : NotOrganiser();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventFormViewModel model)
    {
        if (!IsOrganiser) return NotOrganiser();
        if (!ModelState.IsValid) return View(model);
        try
        {
            using var response = await _api.PostAsync("api/events", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Event created. Add a category so participants can enrol.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", await ErrorAsync(response));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            ModelState.AddModelError("", ConnectionError(ex));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsOrganiser) return NotOrganiser();
        try
        {
            var evt = await OwnedEventAsync(id);
            if (evt == null) return NotFound();
            ViewBag.EventID = id;
            return View(new EventFormViewModel
            {
                EventName = evt.EventName,
                Description = evt.Description,
                EventDate = evt.EventDate,
                StartTime = evt.StartTime,
                Location = evt.Location,
                DistanceKm = evt.DistanceKm,
                EventType = evt.EventType,
                MaximumParticipants = evt.MaximumParticipants,
                Status = evt.Status
            });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["ErrorMessage"] = ConnectionError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EventFormViewModel model)
    {
        if (!IsOrganiser) return NotOrganiser();
        ViewBag.EventID = id;
        if (!ModelState.IsValid) return View(model);
        try
        {
            if (await OwnedEventAsync(id) == null) return NotFound();
            using var response = await _api.PutAsync($"api/events/{id}", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Event updated.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", await ErrorAsync(response));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            ModelState.AddModelError("", ConnectionError(ex));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Categories(int eventId)
    {
        if (!IsOrganiser) return NotOrganiser();
        try
        {
            var evt = await OwnedEventAsync(eventId);
            if (evt == null) return NotFound();
            ViewBag.EventID = eventId;
            ViewBag.EventName = evt.EventName;
            using var response = await _api.GetAsync($"api/events/{eventId}/categories");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ErrorAsync(response);
                return View(new List<EventCategoryViewModel>());
            }
            return View(await response.Content.ReadFromJsonAsync<List<EventCategoryViewModel>>() ?? new());
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["ErrorMessage"] = ConnectionError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddCategory(int eventId)
    {
        if (!IsOrganiser) return NotOrganiser();
        try
        {
            if (await OwnedEventAsync(eventId) == null) return NotFound();
            return View(new CategoryFormViewModel { EventID = eventId, DistanceKm = 5 });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["ErrorMessage"] = ConnectionError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(CategoryFormViewModel model)
    {
        if (!IsOrganiser) return NotOrganiser();
        if (!ModelState.IsValid) return View(model);
        try
        {
            if (await OwnedEventAsync(model.EventID) == null) return NotFound();
            using var response = await _api.PostAsync($"api/events/{model.EventID}/categories", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category saved.";
                return RedirectToAction(nameof(Categories), new { eventId = model.EventID });
            }
            ModelState.AddModelError("", await ErrorAsync(response));
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            ModelState.AddModelError("", ConnectionError(ex));
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Enrolments(int eventId)
    {
        if (!IsOrganiser) return NotOrganiser();
        try
        {
            var evt = await OwnedEventAsync(eventId);
            if (evt == null) return NotFound();
            ViewBag.EventName = evt.EventName;
            ViewBag.EventID = eventId;
            using var response = await _api.GetAsync($"api/events/{eventId}/enrollments");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = await ErrorAsync(response);
                return View(new List<OrganiserEnrollmentViewModel>());
            }
            return View(await response.Content.ReadFromJsonAsync<List<OrganiserEnrollmentViewModel>>() ?? new());
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["ErrorMessage"] = ConnectionError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Results(int eventId)
    {
        if (!IsOrganiser) return NotOrganiser();
        try
        {
            var evt = await OwnedEventAsync(eventId);
            if (evt == null) return NotFound();
            using var entriesResponse = await _api.GetAsync($"api/events/{eventId}/enrollments");
            using var resultsResponse = await _api.GetAsync($"api/events/{eventId}/results");
            if (!entriesResponse.IsSuccessStatusCode || !resultsResponse.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = !entriesResponse.IsSuccessStatusCode
                    ? await ErrorAsync(entriesResponse) : await ErrorAsync(resultsResponse);
                return RedirectToAction(nameof(Index));
            }
            return View(new EventResultsPageViewModel
            {
                EventID = eventId,
                EventName = evt.EventName,
                Enrollments = await entriesResponse.Content.ReadFromJsonAsync<List<OrganiserEnrollmentViewModel>>() ?? new(),
                Results = await resultsResponse.Content.ReadFromJsonAsync<List<RaceResultViewModel>>() ?? new()
            });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["ErrorMessage"] = ConnectionError(ex);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(RecordResultFormViewModel model)
    {
        if (!IsOrganiser) return NotOrganiser();
        if (model.EventID <= 0 || model.EnrollmentID <= 0) return BadRequest();
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Enter a valid finishing time and position.";
            return RedirectToAction(nameof(Results), new { eventId = model.EventID });
        }
        try
        {
            if (await OwnedEventAsync(model.EventID) == null) return NotFound();
            using var response = await _api.PostAsync($"api/enrollments/{model.EnrollmentID}/result", new
            {
                model.FinishTime, model.OverallPosition, model.CategoryPosition
            });
            TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] = response.IsSuccessStatusCode
                ? "Race result recorded." : await ErrorAsync(response);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            TempData["ErrorMessage"] = ConnectionError(ex);
        }
        return RedirectToAction(nameof(Results), new { eventId = model.EventID });
    }
}
