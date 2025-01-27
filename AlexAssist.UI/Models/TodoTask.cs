using System;
using System.Collections.Generic;

namespace AlexAssist.UI.Models
{
    public class TodoTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Priority { get; set; } = "Medium";
        public List<string> Tags { get; set; } = new();
        public string Category { get; set; } = "General";
    }
} 