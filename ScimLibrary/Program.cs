using Microsoft.OpenApi.Models;
using ScimLibrary.Models;
using ScimLibrary.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using ScimLibrary.Factories;
using ScimAPI.Repository;
using ScimAPI.Demo; // Corrected namespace

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();  // Required for minimal APIs
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "An example API for Swagger integration"
    });
});

// Register the demo repository
builder.Services.AddScoped<IScimUserRepository, InMemoryDatabaseRepository>();
builder.Services.AddScoped<IScimGroupRepository, InMemoryDatabaseRepository>();

builder.Services.AddScoped<IScimErrorFactory, ScimErrorFactory>();


// Register the service
builder.Services.AddScoped<ScimUserService>();
builder.Services.AddScoped<ScimGroupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Swagger in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Access via root URL (localhost:5000/)
    });
}

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
