using System.Threading.RateLimiting;
using DbQLBH_65132292.Data;
using DbQLBH_65132292.Hubs;
using DbQLBH_65132292.Infrastructure;
using DbQLBH_65132292.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// Keep a single console logger to avoid duplicated log lines in terminal/debug sessions.
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

// Thêm EF Core & kết nối
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbQLBH_65132292Connection")));

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.Cookie.Name = "webqlbh.admin";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization(options =>
{
    var adminPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin")
        .Build();

    options.AddPolicy("AdminOnly", adminPolicy);
    options.FallbackPolicy = adminPolicy;
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync("{\"message\":\"Too many requests\"}", token);
    };

    options.AddPolicy("ApiPolicy", httpContext =>
    {
        var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IGiaoDichLookupService, GiaoDichLookupService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<DashboardRealtimeBroadcastService>();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("db");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
var app = builder.Build();

var requestedUrls = CollectRequestedUrls(app, builder);

if (requestedUrls.Count > 0)
{
    var resolvedUrls = ResolvePortConflicts(requestedUrls, app.Logger);
    app.Urls.Clear();
    foreach (var url in resolvedUrls)
    {
        app.Urls.Add(url);
    }
}

// Middleware mặc định
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireRateLimiting("ApiPolicy");
app.MapHealthChecks("/health").AllowAnonymous();
app.MapHub<DashboardHub>("/hubs/dashboard");

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();

static IReadOnlyList<string> ResolvePortConflicts(IEnumerable<string> requestedUrls, ILogger logger)
{
    var listeners = IPGlobalProperties.GetIPGlobalProperties()
        .GetActiveTcpListeners()
        .Select(endpoint => endpoint.Port)
        .ToHashSet();

    var chosenPorts = new HashSet<int>();
    var resolvedUrls = new List<string>();

    foreach (var rawUrl in requestedUrls)
    {
        if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out var uri) || uri.Port <= 0 || !IsLocalHost(uri.Host))
        {
            resolvedUrls.Add(rawUrl);
            continue;
        }

        var targetPort = uri.Port;
        if (!listeners.Contains(targetPort) && !chosenPorts.Contains(targetPort))
        {
            chosenPorts.Add(targetPort);
            resolvedUrls.Add(rawUrl);
            continue;
        }

        var fallbackPort = FindAvailableTcpPort();
        chosenPorts.Add(fallbackPort);

        var fallbackUrl = new UriBuilder(uri)
        {
            Port = fallbackPort
        }.Uri.ToString().TrimEnd('/');

        logger.LogWarning(
            "Port {Port} in URL {Url} is already in use. Falling back to {FallbackUrl}.",
            targetPort,
            rawUrl,
            fallbackUrl);

        resolvedUrls.Add(fallbackUrl);
    }

    return resolvedUrls;
}

static bool IsLocalHost(string host)
{
    if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
    {
        return true;
    }

    return IPAddress.TryParse(host, out var address) && IPAddress.IsLoopback(address);
}

static int FindAvailableTcpPort()
{
    var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    try
    {
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
    finally
    {
        listener.Stop();
    }
}

static List<string> CollectRequestedUrls(WebApplication app, WebApplicationBuilder builder)
{
    var rawSources = new List<string?>
    {
        app.Urls.Count > 0 ? string.Join(';', app.Urls) : null,
        builder.WebHost.GetSetting("urls"),
        builder.Configuration["urls"],
        app.Configuration["urls"],
        builder.Configuration["applicationUrl"],
        app.Configuration["applicationUrl"],
        Environment.GetEnvironmentVariable("ASPNETCORE_URLS"),
        Environment.GetEnvironmentVariable("DOTNET_URLS"),
        Environment.GetEnvironmentVariable("URLS"),
        GetLaunchProfileUrls(builder.Environment.ContentRootPath)
    };

    var urls = new List<string>();
    var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    foreach (var raw in rawSources)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            continue;
        }

        var parts = raw.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            if (seen.Add(part))
            {
                urls.Add(part);
            }
        }
    }

    return urls;
}

static string? GetLaunchProfileUrls(string contentRootPath)
{
    var launchSettingsPath = Path.Combine(contentRootPath, "Properties", "launchSettings.json");
    if (!File.Exists(launchSettingsPath))
    {
        return null;
    }

    try
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(launchSettingsPath));
        if (!doc.RootElement.TryGetProperty("profiles", out var profilesElement) || profilesElement.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var activeProfile = Environment.GetEnvironmentVariable("DOTNET_LAUNCH_PROFILE");
        if (!string.IsNullOrWhiteSpace(activeProfile)
            && profilesElement.TryGetProperty(activeProfile, out var activeProfileElement)
            && activeProfileElement.TryGetProperty("applicationUrl", out var activeProfileUrlElement))
        {
            return activeProfileUrlElement.GetString();
        }

        foreach (var profile in profilesElement.EnumerateObject())
        {
            if (profile.Value.TryGetProperty("applicationUrl", out var fallbackUrlElement))
            {
                return fallbackUrlElement.GetString();
            }
        }
    }
    catch
    {
        return null;
    }

    return null;
}