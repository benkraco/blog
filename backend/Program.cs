using backend.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using backend.Repositories;
using backend.Services;
using backend.Services.Storage;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Blog.Auth";

        options.LoginPath = "/api/auth/login";

        options.LogoutPath = "/api/auth/logout";

        options.ExpireTimeSpan = TimeSpan.FromDays(30);

        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;

        options.Cookie.SameSite = SameSiteMode.None;

        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://blog.benkraco.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Database
builder.Services.AddSingleton<DbConnectionFactory>();

// Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<TagRepository>();
builder.Services.AddScoped<PostTagRepository>();
builder.Services.AddScoped<ImageRepository>();

// Services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<TagService>();
builder.Services.AddScoped<PostTagService>();
builder.Services.AddScoped<ImageService>();

builder.Services.AddScoped<IStorageService, CloudflareR2StorageService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();