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

            // Ensure database and tables are created
            try
            {
                Database.OpenConnection();
                using var command = Database.GetDbConnection().CreateCommand();
                
                // Create Tasks table
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        CreatedAt TEXT NOT NULL,
                        DueDate TEXT,
                        IsCompleted INTEGER NOT NULL,
                        Priority TEXT NOT NULL,
                        Tags TEXT,
                        Category TEXT
                    );";
                command.ExecuteNonQuery();

                // Create DiaryEntries table
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS DiaryEntries (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        CreatedAt TEXT NOT NULL,
                        Title TEXT NOT NULL,
                        Content TEXT,
                        Mood TEXT,
                        Tags TEXT,
                        CodeSnippets TEXT,
                        IsFavorite INTEGER NOT NULL
                    );";
                command.ExecuteNonQuery();

                // Create Quotes table
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Quotes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Text TEXT NOT NULL,
                        Author TEXT,
                        Category TEXT,
                        IsFavorite INTEGER NOT NULL
                    );";
                command.ExecuteNonQuery();

                // Create CodingSessions table
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS CodingSessions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        StartTime TEXT NOT NULL,
                        EndTime TEXT,
                        Project TEXT,
                        Description TEXT,
                        Tags TEXT
                    );";
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating database: {ex.Message}");
                throw; // Re-throw to ensure the error is not silently ignored
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