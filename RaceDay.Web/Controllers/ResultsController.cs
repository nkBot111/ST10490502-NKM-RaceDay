using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Web.Controllers;

public class ResultsController : Controller
{
    private readonly RaceDayApiService _api;
    public ResultsController(RaceDayApiService api) => _api = api;

    [HttpGet]
    public async Task<IActionResult> My()
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Login", "Account");
        try
        {
            using var response = await _api.GetAsync("api/results/me");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Results are not available. Please sign in again if your session expired.";
                return View(new List<RaceResultViewModel>());
            }
            return View(await response.Content.ReadFromJsonAsync<List<RaceResultViewModel>>() ?? new());
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            ViewBag.Error = "The API is unavailable. Check that it is running.";
            return View(new List<RaceResultViewModel>());
        }
    }
}
