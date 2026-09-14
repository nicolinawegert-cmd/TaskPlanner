using TaskPlanner.Api.Models;

namespace TaskPlanner.Api.Services;

public class TaskService
{
  private readonly List<TaskItem> _tasks = new();

  public IEnumerable<TaskItem> GetAll()
  {
    return _tasks;
  }
}