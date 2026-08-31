using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using WEB_453504_ASP_NET.UI.Models;
using WEB_453504_ASP_NET.UI.Services.Authentification;
using WEB_453504_ASP_NET.UI.Services.FileSevice;
using Microsoft.AspNetCore.Authorization;

namespace WEB_453504_ASP_NET.UI.Controllers;

public class Account(
    IHttpContextAccessor contextAccessor,
    HttpClient httpClient,
    ITokenAccessor tokenAccessor,
    IOptions<KeycloakData> options,
    IFileService fileService,
    ILogger<Account> logger
) : Controller
{
    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterUserViewModel());
    }

    // POST: /Account/Register
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterUserViewModel user)
    {
        logger.LogInformation($"-----> Registering user: {user.Email}");
        if (!ModelState.IsValid)
        {
            return View(user);
        }

        if (user == null)
        {
            return BadRequest();
        }

        try
        {
            // Получение токена клиента для доступа к Admin API Keycloak
            await tokenAccessor.SetAuthorizationHeaderAsync(httpClient, true);
        }
        catch (Exception ex)
        {
            logger.LogError("-----> Error occurred while setting authorization header: {0}", ex.Message);
            return Unauthorized();
        }

        var avatarUrl = "/Images/default-profile-picture.png";

        // Сохранение аватара, если он был передан
        if (user.Avatar != null)
        {
            avatarUrl = await fileService.SaveFileAsync(user.Avatar);
        }

        // Подготовка модели нового пользователя
        var newUser = new CreateUserModel
        {
            Email = user.Email,
            Username = user.Email,
            Enabled = true,
            EmailVerified = true
        };

        newUser.Attributes.Add("avatar", avatarUrl);
        newUser.Credentials.Add(new UserCredentials
        {
            Value = user.Password
        });

        // Эндпоинт создания пользователя в Keycloak Admin REST API
        var requestUri = $"{options.Value.Host}/admin/realms/{options.Value.Realm}/users";

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var userData = JsonSerializer.Serialize(newUser, serializerOptions);
        var content = new StringContent(userData, Encoding.UTF8, "application/json");

        // Отправка запроса на создание пользователя
        var response = await httpClient.PostAsync(requestUri, content);
        logger.LogInformation($"-----> User creation response: {response.ToString()}");


        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Home");
        }
        
        return BadRequest(response.StatusCode);
    }

    // GET: /Account/Login
    public async Task Login()
    {
        await HttpContext.ChallengeAsync(
            "keycloak",
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            });
    }

    // POST: /Account/Logout
    [HttpPost]
    public async Task Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync("keycloak", new AuthenticationProperties
        {
            RedirectUri = Url.Action("Index", "Home")
        });
    }
}

public class CreateUserModel
{
    public Dictionary<string, string> Attributes { get; set; } = new();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    public bool EmailVerified { get; set; } = true;
    public List<UserCredentials> Credentials { get; set; } = new();
}

public class UserCredentials
{
    public string Type { get; set; } = "password";
    public bool Temporary { get; set; } = false;
    public string Value { get; set; } = string.Empty;
}