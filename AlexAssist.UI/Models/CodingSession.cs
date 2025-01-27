using System;
using System.Collections.Generic;

namespace AlexAssist.UI.Models
{
    public class CodingSession
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : DateTime.Now - StartTime;
        public string Project { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive => !EndTime.HasValue;
        public List<string> Tags { get; set; } = new();
    }
} 