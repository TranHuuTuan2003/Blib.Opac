using System.Globalization;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;

using KMS.Shared.DTOs.Menu;
using KMS.Shared.Models.Tenant;
using KMS.Web.Authentication;
using KMS.Web.Common;
using KMS.Web.Common.Lang;
using KMS.Web.Core;
using KMS.Web.Helpers;
using KMS.Web.Middlewares;
using KMS.Web.Models.JsonConfig;

using Serilog;

using UC.Core.Interfaces;
using Uc.Services.Services.Lms;
using KMS.Shared.DTOs.Footer;

LogService.EnsureDirectoriesExist();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration)
);

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

string AllowOrigins = "TrustedOrigins";
string allowedHosts = "*";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins,
    builder =>
    {
        builder.WithOrigins(allowedHosts).AllowAnyHeader().AllowAnyMethod();
    });
});

int sessionTimeout = int.Parse(config.GetSection("SessionTimeout").Value);
string locationValue = config.GetSection("AppConfig:Location").Value;

builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
// builder.Services.AddScoped<KMS.Web.Services.ESearch.IService, KMS.Web.Services.ESearch.Service>();
builder.Services.AddScoped<KMS.Web.Services.Auth.IService, KMS.Web.Services.Auth.Service>();
builder.Services.AddScoped<KMS.Web.Services.Search.IService, KMS.Web.Services.Search.Service>();
builder.Services.AddScoped<KMS.Web.Services.Document.IService, KMS.Web.Services.Document.Service>();
builder.Services.AddScoped<KMS.Web.Services.FacetFilter.IService, KMS.Web.Services.FacetFilter.Service>();
builder.Services.AddScoped<KMS.Web.Services.PLog.IService, KMS.Web.Services.PLog.Service>();
builder.Services.AddScoped<KMS.Web.Services.Chatbot.IService, KMS.Web.Services.Chatbot.Service>();
builder.Services.AddScoped<KMS.Web.Services.Menu.IService, KMS.Web.Services.Menu.Service>();
builder.Services.AddScoped<KMS.Web.Services.Home.IService, KMS.Web.Services.Home.Service>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

var jwtTokenConfig = config.GetSection("IdentityServerAuthentication").Get<IdentityServerAuthentication>();
builder.Services.AddScoped<AppConfigHelper>();
builder.Services.AddScoped<ApiHelper>();
builder.Services.AddScoped<AuthHelper>();
builder.Services.AddScoped<IManifestStaticFiles, ManifestStaticFiles>();
builder.Services.AddSingleton<IJwtAuthManager>(provider => new JwtAuthManager(jwtTokenConfig));
builder.Services.AddSingleton<IDictionary<string, List<string>>>(new Dictionary<string, List<string>>());
builder.Services.AddSingleton<JwtTokenHelper>();
// builder.Services.AddSingleton<ViewCountManager>();

builder.Services.AddSingleton(new LangResource());
builder.Services.AddSingleton<IStringLocalizerFactory, TenantJsonStringLocalizerFactory>();
builder.Services.AddLocalization();

//builder.Services.AddHostedService<SyncBackgroundService>();

builder.Services.AddSignalR(options => { options.KeepAliveInterval = TimeSpan.FromSeconds(5); }).AddMessagePackProtocol();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/keys"))
    .SetApplicationName("Ucvn-Opac");


#region Register Localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

CultureInfo[] cultureInfos = { new CultureInfo("vi"), new CultureInfo("en") };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureInfos[0]);
    options.SupportedCultures = cultureInfos;
    options.SupportedUICultures = cultureInfos;
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),
        new CookieRequestCultureProvider()
    };
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    var _appConfigHelper = builder.Services.BuildServiceProvider().GetRequiredService<AppConfigHelper>();
    var hash_key = _appConfigHelper.GetHashKey();
    var tenant_code = _appConfigHelper.GetTenantCode();
    options.Cookie.Name = _appConfigHelper.GenerateHMACHash(hash_key, $"{tenant_code}_token");
    options.ExpireTimeSpan = TimeSpan.FromMinutes(jwtTokenConfig.AccessTokenExpiration);
    options.LoginPath = locationValue + "/het-phien-su-dung";
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = jwtTokenConfig.RequireHttpsMetadata;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtTokenConfig.Issuer,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
        ValidAudience = jwtTokenConfig.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddScoped(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<JsonConfigCacheService<List<SearchCriteria>>>>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var tenantCode = sp.GetRequiredService<AppConfigHelper>().GetTenantCode();
    string path = Path.Combine(env.WebRootPath, "configs", tenantCode, "search_criteria", "search_criteria.json");
    return new JsonConfigCacheService<List<SearchCriteria>>("SearchCriteria", path, cache, logger);
});

builder.Services.AddScoped(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<JsonConfigCacheService<TenantConfig>>>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var tenantCode = sp.GetRequiredService<AppConfigHelper>().GetTenantCode();
    string path = Path.Combine(env.WebRootPath, "configs", tenantCode, "tenants", "tenants.json");
    return new JsonConfigCacheService<TenantConfig>("TenantConfig", path, cache, logger);
});

builder.Services.AddScoped(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<JsonConfigCacheService<List<Menu>>>>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var tenantCode = sp.GetRequiredService<AppConfigHelper>().GetTenantCode();
    string path = Path.Combine(env.WebRootPath, "configs", tenantCode, "menu", "menu.json");
    return new JsonConfigCacheService<List<Menu>>("Menu", path, cache, logger);
});

builder.Services.AddScoped(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<JsonConfigCacheService<Footer>>>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var tenantCode = sp.GetRequiredService<AppConfigHelper>().GetTenantCode();
    string path = Path.Combine(env.WebRootPath, "configs", tenantCode, "footer", "footer.json");
    return new JsonConfigCacheService<Footer>("Footer", path, cache, logger);
});

builder.Services.AddScoped(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<JsonConfigCacheService<Dictionary<string, string>>>>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    string path = Path.Combine(env.WebRootPath, "public", "manifest", "manifest.json");
    return new JsonConfigCacheService<Dictionary<string, string>>("Manifest", path, cache, logger);
});

builder.Services.AddScoped(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<JsonConfigCacheService<KmsParam>>>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    var tenantCode = sp.GetRequiredService<AppConfigHelper>().GetTenantCode();
    string path = Path.Combine(env.WebRootPath, "configs", tenantCode, "kms_params", "kms_params.json");
    return new JsonConfigCacheService<KmsParam>("KmsParam", path, cache, logger);
});

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews().AddRazorOptions(options =>
{
    options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
}).AddRazorRuntimeCompilation();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "kms-session";
});

#endregion

var app = builder.Build();

app.UseSession();

app.UseMiddleware<AppConfigResolutionMiddleware>();
// app.UseMiddleware<ApiKeySetupMiddleware>();
if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<BlockDevelopmentPathMiddleware>();
}
app.UseMiddleware<SessionSetupMiddleware>();

app.UseCors(AllowOrigins);

app.UseRequestLocalization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();

string root = Path.Combine(Directory.GetCurrentDirectory(), "static");
if (!File.Exists(root))
{
    Directory.CreateDirectory(root);
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// app.UseRateLimiter();

var fsOptions = new FileServerOptions();
fsOptions.StaticFileOptions.OnPrepareResponse = (context) =>
{
    if (context.File.Name.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
    {
        context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
    }
    else
    {
        context.Context.Response.Headers["Cache-Control"] = "public,max-age=31536000,immutable";
    }
    // context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
};
app.UseFileServer(fsOptions);

app.UseStaticFiles();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=CollectionManagement}/{action=Index}/{id?}");

// app.MapDynamicControllerRoute<DynamicRouteTransformer>("{page}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.UseStatusCodePagesWithReExecute(ConstLocation.value + "/Error/SourcesNotFound");

//HandleRedirects(app);

ConstLocation.Initialize(builder.Configuration);

app.Logger.LogInformation("Ứng dụng đang khởi động...");

app.Lifetime.ApplicationStarted.Register(() =>
{
    foreach (var url in app.Urls)
    {
        app.Logger.LogInformation($"App is running at {url}");
    }
});

app.Run();

#region HandleRedirects
void HandleRedirects(IApplicationBuilder app)
{
    // Old Url, New Url
    var redirects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {

    };

    app.Use(async (context, next) =>
    {

        if (redirects != null)
        {
            var url = context.Request.Path.Value;

            if (!string.IsNullOrEmpty(url) && redirects.TryGetValue(url, out var redirectUrl))
            {
                context.Response.Redirect(redirectUrl);
                return;
            }
        }
        await next();
    });
}
#endregion
