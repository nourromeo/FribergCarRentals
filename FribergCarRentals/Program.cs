using FribergCarRentals;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// MVC Views
builder.Services.AddControllersWithViews();

// Session & HttpContext
builder.Services.AddDistributedMemoryCache();

// Give the frontend a distinct session cookie name to avoid collisions with API
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "FribergCarRentals.Frontend.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

builder.Services.AddHttpContextAccessor();

// Authorized handler to include JWT token automatically
builder.Services.AddTransient<AuthorizedHttpClientHandler>();

// API base URL with Authorization handler
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7049/api/");
})
.AddHttpMessageHandler<AuthorizedHttpClientHandler>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    var jwtSettings = builder.Configuration.GetSection("JwtSettings");

    var issuer = jwtSettings["Issuer"];
    var audience = jwtSettings["Audience"];
    var key = jwtSettings["Key"];
    var decodedKey = Encoding.UTF8.GetBytes(key);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(decodedKey),
        RoleClaimType = ClaimTypes.Role,   
        NameClaimType = ClaimTypes.Name   
    };
});


var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
