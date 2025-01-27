using System;
using System.Collections.Generic;

namespace AlexAssist.UI.Models
{
    public class CodeSnippet
    {
        public string Language { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class DiaryEntry
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Mood { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public List<CodeSnippet> CodeSnippets { get; set; } = new();
        public bool IsFavorite { get; set; }
    }
} 