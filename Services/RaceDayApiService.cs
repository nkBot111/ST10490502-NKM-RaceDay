
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Web.Services
{
    public class RaceDayApiService
    {
        private const string ApiCookieSessionKey =
            "RaceDay.ApiSessionCookie";

        private const string ApiCookieName =
            ".AspNetCore.Session";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceDayApiService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Retrieve data from the API.
        public Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            return SendAsync(HttpMethod.Get, endpoint);
        }

        // POST: Send data to the API.
        public Task<HttpResponseMessage> PostAsync<T>(
            string endpoint, T data)
        {
            return SendAsync(
                HttpMethod.Post,
                endpoint,
                JsonContent.Create(data));
        }

        // PUT: Update existing data through the API.
        public Task<HttpResponseMessage> PutAsync<T>(
            string endpoint, T data)
        {
            return SendAsync(
                HttpMethod.Put,
                endpoint,
                JsonContent.Create(data));
        }

        // DELETE: Delete or cancel a resource through the API.
        public Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            return SendAsync(HttpMethod.Delete, endpoint);
        }

        // Remove the stored API session cookie during logout.
        public void ClearApiSession()
        {
            _httpContextAccessor.HttpContext?
                .Session.Remove(ApiCookieSessionKey);
        }

        // Send an API request and preserve the current user's
        // API authentication cookie.
        private async Task<HttpResponseMessage> SendAsync(
            HttpMethod method,
            string endpoint,
            HttpContent? content = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint) ||
                !Uri.TryCreate(
                    endpoint,
                    UriKind.Relative,
                    out _) ||
                !endpoint.StartsWith(
                    "api/",
                    StringComparison.OrdinalIgnoreCase))
            {
                content?.Dispose();

                throw new ArgumentException(
                    "The endpoint must be a relative API path.",
                    nameof(endpoint));
            }

            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException(
                    "The current HTTP context is unavailable.");

            var client = _httpClientFactory.CreateClient(
                "RaceDayApi");

            using var request = new HttpRequestMessage(
                method, endpoint);

            request.Content = content;

            string? storedCookie =
                httpContext.Session.GetString(
                    ApiCookieSessionKey);

            if (!string.IsNullOrWhiteSpace(storedCookie))
            {
                request.Headers.TryAddWithoutValidation(
                    "Cookie",
                    storedCookie);
            }

            var response = await client.SendAsync(request);

            // The Part 2 API issues an ASP.NET Core session cookie
            // when the user logs in successfully.
            if (response.Headers.TryGetValues(
                "Set-Cookie", out var cookieHeaders))
            {
                foreach (string header in cookieHeaders)
                {
                    string cookiePair =
                        header.Split(';', 2)[0].Trim();

                    if (cookiePair.StartsWith(
                        ApiCookieName + "=",
                        StringComparison.Ordinal))
                    {
                        string cookieValue = cookiePair.Substring(
                            ApiCookieName.Length + 1);

                        if (string.IsNullOrEmpty(cookieValue))
                        {
                            httpContext.Session.Remove(
                                ApiCookieSessionKey);
                        }
                        else
                        {
                            httpContext.Session.SetString(
                                ApiCookieSessionKey,
                                cookiePair);
                        }
                    }
                }
            }

            return response;
        }
    }
}
