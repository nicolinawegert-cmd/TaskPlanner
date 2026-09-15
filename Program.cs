using TaskPlanner.Api.Services;
using TaskPlanner.Api.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskPlanner.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddScoped<TaskService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Endpoints
app.MapGet("/api/tasks", async (TaskService taskService) =>
{
    var tasks = await taskService.GetAllAsync();

    return Results.Ok(tasks);
});

app.MapPost("/api/tasks", async (TaskItem task, TaskService taskService) =>
{
    if (string.IsNullOrWhiteSpace(task.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    var createdTask = await taskService.AddAsync(task);

    return Results.Created($"/api/tasks/{createdTask.Id}", createdTask);
});

app.MapPut("/api/tasks/{id:int}", async (int id, TaskItem task, TaskService taskService) =>
{
    if (string.IsNullOrWhiteSpace(task.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    var updatedTask = await taskService.UpdateAsync(id, task);

    if (updatedTask is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(updatedTask);
});

app.MapPost("/api/tasks/{id:int}/file", async (int id, IFormFile file, TaskService taskService) =>
{
    var task = await taskService.GetByIdAsync(id);

    if (task is null)
    {
        return Results.NotFound();
    }

    if (file.Length == 0)
    {
        return Results.BadRequest("File is empty.");
    }

    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    Directory.CreateDirectory(uploadsFolder);

    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
    var filePath = Path.Combine(uploadsFolder, fileName);

    await using var stream = new FileStream(filePath, FileMode.Create);
    await file.CopyToAsync(stream);

    var updatedTask = await taskService.UpdateFileAsync(task, fileName);

    return Results.Ok(updatedTask);
});

app.Run();