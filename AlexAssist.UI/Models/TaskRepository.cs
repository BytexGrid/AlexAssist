using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AlexAssist.UI.Models
{
    public class TaskRepository : IDisposable
    {
        private readonly AppDbContext _context;

        public TaskRepository()
        {
            _context = new AppDbContext();
        }

        public List<TodoTask> GetAllTasks()
        {
            try
            {
                return _context.Tasks.OrderByDescending(t => t.DueDate).ToList();
            }
            catch
            {
                return new List<TodoTask>();
            }
        }

        public List<TodoTask> GetTasksDueToday()
        {
            try
            {
                var today = DateTime.Today;
                return _context.Tasks
                    .Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == today)
                    .OrderBy(t => t.DueDate)
                    .ToList();
            }
            catch
            {
                return new List<TodoTask>();
            }
        }

        public void AddTask(TodoTask task)
        {
            try
            {
                _context.Tasks.Add(task);
                _context.SaveChanges();
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public void UpdateTask(TodoTask task)
        {
            try
            {
                var existing = _context.Tasks.Find(task.Id);
                if (existing != null)
                {
                    _context.Entry(existing).CurrentValues.SetValues(task);
                    existing.Tags = task.Tags;
                    _context.SaveChanges();
                }
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public void DeleteTask(TodoTask task)
        {
            try
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
            catch
            {
                // Log error or handle appropriately
            }
        }

        public void ToggleTaskCompletion(TodoTask task)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Repository: Finding task {task.Id}");
                var existing = _context.Tasks.Find(task.Id);
                if (existing != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Repository: Found task, current completion status: {existing.IsCompleted}");
                    existing.IsCompleted = !existing.IsCompleted;
                    _context.SaveChanges();
                    System.Diagnostics.Debug.WriteLine($"Repository: Task updated, new completion status: {existing.IsCompleted}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Repository: Task not found in database");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Repository Error: {ex.Message}");
            }
        }

        public int GetCompletedTasksCount()
        {
            return _context.Tasks.Count(t => t.IsCompleted);
        }

        public int GetCompletionStreak()
        {
            var tasks = _context.Tasks
                .Where(t => t.IsCompleted)
                .OrderByDescending(t => t.DueDate)
                .ToList();

            if (!tasks.Any())
                return 0;

            int streak = 0;
            DateTime? lastDate = null;

            foreach (var task in tasks)
            {
                if (!task.DueDate.HasValue)
                    continue;

                var taskDate = task.DueDate.Value.Date;

                if (!lastDate.HasValue)
                {
                    lastDate = taskDate;
                    streak = 1;
                    continue;
                }

                // If this task was completed on the previous day, increment streak
                if (taskDate == lastDate.Value.AddDays(-1))
                {
                    streak++;
                    lastDate = taskDate;
                }
                else if (taskDate != lastDate.Value) // Different day, not consecutive
                {
                    break;
                }
            }

            return streak;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
} 