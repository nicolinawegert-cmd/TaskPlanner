namespace TaskPlanner.Api.Models;

public class TaskItem
{
  public int Id { get; set; }

  public string Title { get; set; } = string.Empty;

  public string Description { get; set; } = string.Empty;

  public TaskItemStatus Status { get; set; } = TaskItemStatus.NotStarted;

  public DateTime? DueDate { get; set; }

  public string? FileName { get; set; }
}