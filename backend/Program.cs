using DivHunt;
using DivHunt.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        var allowedOrigins = builder.Configuration.GetValue<string>("AllowedOrigins")?.Split(", "); 
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins(allowedOrigins ?? new [] { "http://localhost:5173" }).AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("Default");

app.MapHub<MainHub>("/hub").AllowAnonymous();

app.Run();