using TaskPlanner.Api.Services;
using TaskPlanner.Api.Models;
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddSingleton<TaskService>();

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
    var createdTask = taskService.Add(task);

    return Results.Created($"/api/tasks/{createdTask.Id}", createdTask);
});

app.Run();