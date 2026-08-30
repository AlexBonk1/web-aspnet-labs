

using Microsoft.OpenApi;
namespace WEB_453504_ASP_NET.API;
public static class DependencyInjection {
  public static IServiceCollection AddSwaggerWithKeycloak(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = builder.Configuration["Swagger:Title"] ?? "Core API",
                Version = builder.Configuration["Swagger:Version"] ?? "v1",
                Description = builder.Configuration["Swagger:Description"] ?? "Core API for EducationSigma",
            });



        var keycloakAuthority = builder.Configuration["Keycloak:Host"]! + "/realms/" + builder.Configuration["Keycloak:Realm"]!;

        options.AddSecurityDefinition(nameof(SecuritySchemeType.OAuth2), new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect scope" },
                    { "profile", "User profile" }
                }
            }
        }
    });       
     options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference(nameof(SecuritySchemeType.OAuth2), doc),
            []
        }
    });

});
    return builder.Services;

}
}