using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StudioRags.API.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StudioRagsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<StudioRagsDbContext>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactFrontend", policy =>
    {
        policy.WithOrigins(
        "http://localhost:5173",
        "http://localhost:5174",
        "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();   // ADDED: lets the backend show the React website
app.UseStaticFiles();    // ADDED: lets the backend show the React website

app.UseCors("ReactFrontend");

app.UseAuthorization();

app.MapControllers();

app.MapIdentityApi<IdentityUser>();
app.MapGet("/api/protected", () =>
{
    return Results.Ok(new
    {
        message = "Authentication successful! You can access this protected API."
    });
})
.RequireAuthorization();

app.MapFallbackToFile("index.html");   // ADDED: opens the React website at the main link

app.Run();
