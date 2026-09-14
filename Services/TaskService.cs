using TaskPlanner.Api.Models;

namespace TaskPlanner.Api.Services;

public class TaskService
{
  private readonly List<TaskItem> _tasks = new();

  public IEnumerable<TaskItem> GetAll()
  {
    return _tasks;
  }

  public TaskItem Add(TaskItem task)
  {
    task.Id = _tasks.Count == 0
        ? 1
        : _tasks.Max(t => t.Id) + 1;

    _tasks.Add(task);

    return task;
  }

  public TaskItem? Update(int id, TaskItem updatedTask)
  {
    var existingTask = _tasks.FirstOrDefault(t => t.Id == id);

    if (existingTask is null)
    {
      return null;
    }

    existingTask.Title = updatedTask.Title;
    existingTask.Description = updatedTask.Description;
    existingTask.Status = updatedTask.Status;
    existingTask.DueDate = updatedTask.DueDate;
    existingTask.FileName = updatedTask.FileName;

    return existingTask;
  }
}