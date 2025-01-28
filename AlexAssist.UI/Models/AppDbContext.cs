using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.IO;

namespace AlexAssist.UI.Models
{
    public class AppDbContext : DbContext
    {
        private static readonly JsonSerializerOptions _jsonOptions = new();
        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "AlexAssist",
            "alexassist.db"
        );
        
        public DbSet<DiaryEntry> DiaryEntries => Set<DiaryEntry>();
        public DbSet<TodoTask> Tasks => Set<TodoTask>();
        public DbSet<Quote> Quotes { get; set; } = null!;
        public DbSet<CodingSession> CodingSessions => Set<CodingSession>();

        public AppDbContext()
        {
            // Ensure database directory exists
            var folder = Path.GetDirectoryName(DbPath);
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Ensure database is created and schema is up to date
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ensuring database created: {ex.Message}");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure DiaryEntry
            modelBuilder.Entity<DiaryEntry>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new List<string>()
                );

            // Configure CodeSnippets
            modelBuilder.Entity<DiaryEntry>()
                .Property(e => e.CodeSnippets)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<CodeSnippet>>(v, _jsonOptions) ?? new List<CodeSnippet>()
                );

            // Configure TodoTask
            modelBuilder.Entity<TodoTask>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new List<string>()
                );

            // Configure CodingSession
            modelBuilder.Entity<CodingSession>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonOptions) ?? new List<string>()
                );
        }
    }
} 