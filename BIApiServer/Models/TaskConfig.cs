using System;

namespace BIApiServer.Models
{
    public class TaskConfig
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Url { get; set; }
        public string Method { get; set; } = "GET";
        public string Parameters { get; set; }
        public double IntervalSeconds { get; set; } = 60;
        public DateTime? LastExecutionTime { get; set; }
        public bool IsEnabled { get; set; } = true;
        public DateTime CreatedTime { get; set; } = DateTime.Now;
    }
} 