public class TaskExecutionHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TaskId { get; set; }
    public DateTime ExecutionTime { get; set; }
    public bool IsSuccess { get; set; }
    public string Response { get; set; }
    public string Error { get; set; }
} 