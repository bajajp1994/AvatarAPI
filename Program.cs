using System.Text.Json.Serialization;
using AvatarAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()    // Allows any origin (you can limit to specific origins)
              .AllowAnyHeader()    // Allows any header
              .AllowAnyMethod();   // Allows any HTTP method (GET, POST, etc.)
    });
});

builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.AddDebug();
    options.SetMinimumLevel(LogLevel.Trace);
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient(); // Add HttpClient factory

var app = builder.Build();

// Enable default file (index.html) serving
app.UseDefaultFiles();  // This will serve 'index.html' automatically if it exists in wwwroot

// Enable static file serving
app.UseStaticFiles();

app.UseCors("AllowAll");

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

// Configure the HTTP request pipeline.
app.MapControllers();
app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
