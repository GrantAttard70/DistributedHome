using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

[Authorize]
public class BookingController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BookingController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("GatewayApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetUserToken());

        var response = await client.GetAsync("/api/bookings");

        if (response.IsSuccessStatusCode)
        {
            var bookings = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>();
            return View(bookings);
        }

        return View(new List<BookingViewModel>());
    }
    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var client = _httpClientFactory.CreateClient("GatewayApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetUserToken());

        var response = await client.GetAsync("/api/bookings/user");

        if (response.IsSuccessStatusCode)
        {
            var bookings = await response.Content.ReadFromJsonAsync<List<BookingViewModel>>();
            return View(bookings);
        }

        TempData["ErrorMessage"] = "Could not load your bookings.";
        return View(new List<BookingViewModel>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new BookingViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookingViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("GatewayApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetUserToken());

        var response = await client.PostAsJsonAsync("/api/bookings", model);

        if (response.IsSuccessStatusCode)
        {
            var booking = await response.Content.ReadFromJsonAsync<BookingViewModel>();
            return RedirectToAction("Payment", new { id = booking.Id });
        }

        ModelState.AddModelError(string.Empty, "Error creating booking");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Payment(Guid id)
    {
        var client = _httpClientFactory.CreateClient("GatewayApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetUserToken());

        var response = await client.GetAsync($"/api/bookings/{id}");

        if (response.IsSuccessStatusCode)
        {
            var booking = await response.Content.ReadFromJsonAsync<BookingViewModel>();
            return View(booking);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ProcessPayment(Guid bookingId)
    {
        var client = _httpClientFactory.CreateClient("GatewayApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GetUserToken());

        var response = await client.PostAsJsonAsync($"/api/payments/process/{bookingId}", new { });

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Payment processed successfully!";
            return RedirectToAction("Index");
        }

        TempData["ErrorMessage"] = "Payment failed. Please try again.";
        return RedirectToAction("Payment", new { id = bookingId });
    }

    private string GetUserToken()
    {
        return User.FindFirst("Token")?.Value;
    }
}