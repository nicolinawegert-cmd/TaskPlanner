using Microsoft.EntityFrameworkCore;
using TaskPlanner.Api.Data;
using TaskPlanner.Api.Models;

namespace TaskPlanner.Api.Services;

public class TaskService
{
  private readonly AppDbContext _context;

  public TaskService(AppDbContext context)
  {
    _context = context;
  }

  public async Task<List<TaskItem>> GetAllAsync()
  {
    return await _context.Tasks.ToListAsync();
  }

  public async Task<TaskItem?> GetByIdAsync(int id)
  {
    return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
  }

  public async Task<TaskItem> AddAsync(TaskItem task)
  {
    _context.Tasks.Add(task);
    await _context.SaveChangesAsync();

    return task;
  }

  public async Task<TaskItem?> UpdateAsync(int id, TaskItem updatedTask)
  {
    var existingTask = await _context.Tasks
        .FirstOrDefaultAsync(t => t.Id == id);

    if (existingTask is null)
    {
      return null;
    }

    existingTask.Title = updatedTask.Title;
    existingTask.Description = updatedTask.Description;
    existingTask.Status = updatedTask.Status;
    existingTask.DueDate = updatedTask.DueDate;

    await _context.SaveChangesAsync();

    return existingTask;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var task = await GetByIdAsync(id);

    if (task is null)
    {
      return false;
    }

    _context.Tasks.Remove(task);
    await _context.SaveChangesAsync();

    return true;
  }

  public async Task<TaskItem> UpdateFileAsync(TaskItem task, string fileName)
  {
    task.FileName = fileName;
    await _context.SaveChangesAsync();

    return task;
  }
}
