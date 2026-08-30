using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WEB_453504_ASP_NET.UI.Models;

namespace WEB_453504_ASP_NET.UI;


public static class DependencyInjection
{
    public static IServiceCollection AddCookiesAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakData = configuration.GetSection("Keycloak").Get<KeycloakData>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "keycloak";
        })
        .AddCookie()
        .AddOpenIdConnect("keycloak", options =>
        {
            options.Authority = $"{keycloakData.Host}/auth/realms/{keycloakData.Realm}";
            options.ClientId = keycloakData.ClientId;
            options.ClientSecret = keycloakData.ClientSecret;
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.Scope.Add("openid");
            options.SaveTokens = true;
            options.RequireHttpsMetadata = false;
            options.MetadataAddress = $"{keycloakData.Host}/realms/{keycloakData.Realm}/.well-known/openid-configuration";
        });
        return services;
    }
}