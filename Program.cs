using TaskPlanner.Api.Services;
using TaskPlanner.Api.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskService>();

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
app.MapGet("/api/tasks", (TaskService taskService) =>
{
    return Results.Ok(taskService.GetAll());
});

app.MapPost("/api/tasks", (TaskItem task, TaskService taskService) =>
{
    if (string.IsNullOrWhiteSpace(task.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    var createdTask = taskService.Add(task);

    return Results.Created($"/api/tasks/{createdTask.Id}", createdTask);
});

app.MapPut("/api/tasks/{id:int}", (int id, TaskItem task, TaskService taskService) =>
{
    if (string.IsNullOrWhiteSpace(task.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    var updatedTask = taskService.Update(id, task);

    if (updatedTask is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(updatedTask);
});

app.MapPost("/api/tasks/{id:int}/file", async (int id, IFormFile file, TaskService taskService) =>
{
    var task = taskService.GetById(id);

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

    task.FileName = fileName;

    return Results.Ok(task);
});

app.Run();