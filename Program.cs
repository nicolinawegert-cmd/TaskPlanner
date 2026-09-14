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

app.Run();