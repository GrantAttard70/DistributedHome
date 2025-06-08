using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CabBookingWeb.ViewModels;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace CabBookingWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loginData = new
                {
                    Email = model.Email,
                    Password = model.Password
                };

                var json = JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Create HTTP client using the factory
                var client = _httpClientFactory.CreateClient("GatewayApi");

                // Call Customer microservice through Gateway
                var response = await client.PostAsync("/api/customers/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var userData = JsonConvert.DeserializeObject<dynamic>(result);

                        // Create claims from microservice response
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userData.email?.ToString() ?? model.Email),
                            new Claim(ClaimTypes.Email, userData.email?.ToString() ?? model.Email),
                            new Claim(ClaimTypes.NameIdentifier, userData.id?.ToString() ?? "0")
                        };

                        // Add token if available
                        if (userData.token != null)
                        {
                            claims.Add(new Claim("access_token", userData.token.ToString()));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        return RedirectToAction("Index", "Home");
                    }
                    catch (JsonException)
                    {
                        // If JSON parsing fails, treat as login failure
                        ModelState.AddModelError("", "Invalid response from server");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login credentials");
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient("GatewayApi");

            var response = await client.PostAsJsonAsync("/api/customers/register", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }
    }

    // Model for deserializing login response
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
    }
}