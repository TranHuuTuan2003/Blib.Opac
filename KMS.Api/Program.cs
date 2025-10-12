using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Dapper;

using KMS.Api.Helpers;
using KMS.Api.Infrastructure.Authentication;
using KMS.Api.Infrastructure.DbContext.master;
using KMS.Api.Infrastructure.Swagger;
using KMS.Api.Services;
using KMS.Api.Services.Search.Logic;
using KMS.Shared.DTOs.Document;
using KMS.Shared.Handlers;

using Serilog;

using UC.Core.Common;
using UC.Core.Helpers;
using UC.Core.Interfaces;
using UC.Core.Models.Ums;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) =>
    loggerConfig.ReadFrom.Configuration(context.Configuration)
);

SqlMapper.AddTypeHandler(new JsonTypeHandler<Ext>());

string AllowOrigins = "TrustedOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins,
    builder =>
    {
        builder.WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
    });
});

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "KMS.Api",
        Version = "v1"
    });

    // Cấu hình header X-API-KEY
    c.AddSecurityDefinition("X-API-KEY", new OpenApiSecurityScheme
    {
        Description = "API Key cần thiết để truy cập API",
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-API-KEY"
                },
                In = ParameterLocation.Header,
            },
            Array.Empty<string>()
        }
    });

    c.CustomSchemaIds(i => i.FullName);
    c.SchemaFilter<SwaggerExcludeFilter>();
});

builder.Services.AddScoped<DbSession>();
builder.Services.AddTransient<UnitOfWork>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserProvider, UserProvider>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddSingleton<AppConfigHelper>();
builder.Services.AddSingleton<ApiHelper>();
builder.Services.AddSingleton<IIntermediateSearchLogic, IntermediateSearchLogic>();

// Service wrapper
builder.Services.AddScoped(typeof(IServiceWrapper), typeof(ServiceWrapper));
var jwtTokenConfig = config.GetSection("IdentityServerAuthentication").Get<IdentityServerAuthentication>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

builder.Services.AddSingleton<IJwtAuthManager>(provider => new JwtAuthManager(jwtTokenConfig));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/keys"))
    .SetApplicationName("Ucvn-Opac");
//builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();
app.UseCors(AllowOrigins);

// app.UseMiddleware<ApiKeyMiddleware>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

if (UcHelper.IsLinux())
{
    builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/keys"))
    .SetApplicationName("Ucvn-Opac");
}

app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseRouting();
// app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

string root = Path.Combine(Directory.GetCurrentDirectory(), "static");
if (!File.Exists(root))
{
    Directory.CreateDirectory(root);
}

app.Logger.LogInformation("Ứng dụng đang khởi động...");

app.Lifetime.ApplicationStarted.Register(() =>
{
    foreach (var url in app.Urls)
    {
        app.Logger.LogInformation($"App is running at {url}");
    }
});

app.Run();
