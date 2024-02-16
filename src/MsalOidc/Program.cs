using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using OicdDemo.Entities;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


var azureSettings = builder.Configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.SameSite = SameSiteMode.None; 
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddOpenIdConnect(options =>
    {
        options.Authority = $"{azureSettings.Authority}";
        options.ClientId = azureSettings.ClientId;
        options.ClientSecret = azureSettings.ClientSecret;
        options.MetadataAddress = azureSettings.MetadataAddress;

        options.ResponseType = "code";
        options.SaveTokens = true;

        //options.CallbackPath = new PathString("/"); // Needs to be registered as callback url by identity provider
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        //options.Scope.Add("email");
        //options.Scope.Add("offline_access"); // Required to get a refresh_token // https://learn.microsoft.com/en-us/azure/active-directory/develop/scopes-oidc#offline_access

        //options.GetClaimsFromUserInfoEndpoint = true;

        //options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        //options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
        //options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
    });

builder.Services.AddAuthorization();


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages((options) =>
{
    options.Conventions.AddPageRoute("/Index", "{*url}");
});

builder.Services.AddCors((options) =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetValue<string>("CorsAllowedOrigin").Split(';'))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.None, 
    Secure = CookieSecurePolicy.Always
});


app.UseCors();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
