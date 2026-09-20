using Microsoft.AspNetCore.Mvc;
using TaskPlanner.Api.Models;
using TaskPlanner.Api.Services;

namespace TaskPlanner.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await _taskService.GetAllAsync();

        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            return BadRequest("Title is required.");
        }

        var createdTask = await _taskService.AddAsync(task);

        return Created($"/api/tasks/{createdTask.Id}", createdTask);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            return BadRequest("Title is required.");
        }

        var updatedTask = await _taskService.UpdateAsync(id, task);

        if (updatedTask is null)
        {
            return NotFound();
        }

        return Ok(updatedTask);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{id:int}/file")]
    public async Task<IActionResult> UploadFile(int id, IFormFile file)
    {
        var task = await _taskService.GetByIdAsync(id);

        if (task is null)
        {
            return NotFound();
        }

        if (file.Length == 0)
        {
            return BadRequest("File is empty.");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var updatedTask = await _taskService.UpdateFileAsync(task, fileName);

        return Ok(updatedTask);
    }
}
