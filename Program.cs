using TaskPlanner.Api.Services;

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

app.Run();