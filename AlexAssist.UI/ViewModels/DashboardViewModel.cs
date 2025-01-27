using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Input;
using AlexAssist.UI.Models;
using System.Linq;
using MaterialDesignThemes.Wpf;

namespace AlexAssist.UI.ViewModels
{
    public class DashboardViewModel : BaseViewModel, IDisposable
    {
        private readonly TaskRepository _taskRepository;
        private readonly DiaryRepository _diaryRepository;
        private string _greeting = string.Empty;
        private string _quote = string.Empty;
        private int _totalTasks;
        private int _completedTasks;
        private int _diaryEntries;
        private int _streak;
        private ObservableCollection<TodoTask> _todaysTasks = new();
        private ObservableCollection<DiaryEntry> _recentDiaryEntries = new();
        private readonly DispatcherTimer _timer;

        public string Greeting
        {
            get => _greeting;
            set => SetField(ref _greeting, value);
        }

        public string Quote
        {
            get => _quote;
            set => SetField(ref _quote, value);
        }

        public int TotalTasks
        {
            get => _totalTasks;
            set => SetField(ref _totalTasks, value);
        }

        public int CompletedTasks
        {
            get => _completedTasks;
            set => SetField(ref _completedTasks, value);
        }

        public int DiaryEntries
        {
            get => _diaryEntries;
            set => SetField(ref _diaryEntries, value);
        }

        public int Streak
        {
            get => _streak;
            set => SetField(ref _streak, value);
        }

        public ObservableCollection<TodoTask> TodaysTasks
        {
            get => _todaysTasks;
            set => SetField(ref _todaysTasks, value);
        }

        public ObservableCollection<DiaryEntry> RecentDiaryEntries
        {
            get => _recentDiaryEntries;
            set => SetField(ref _recentDiaryEntries, value);
        }

        public ICommand ToggleTaskCompletionCommand { get; }
        public ICommand RefreshDataCommand { get; }
        public ICommand NavigateToTaskCommand { get; }
        public ICommand NavigateToDiaryCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public DashboardViewModel()
        {
            _taskRepository = new TaskRepository();
            _diaryRepository = new DiaryRepository();

            // Initialize commands
            ToggleTaskCompletionCommand = new RelayCommand(task => ToggleTaskCompletion((TodoTask)task));
            RefreshDataCommand = new RelayCommand(_ => LoadDashboardData());
            NavigateToTaskCommand = new RelayCommand(task => NavigateToTask((TodoTask)task));
            NavigateToDiaryCommand = new RelayCommand(entry => NavigateToDiary((DiaryEntry)entry));
            DeleteTaskCommand = new RelayCommand(task => DeleteTask((TodoTask)task));

            // Initialize timer for updating greeting
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _timer.Tick += (s, e) => UpdateGreeting();
            _timer.Start();

            UpdateGreeting();
            LoadDashboardData();
            SetRandomQuote();
        }

        private void UpdateGreeting()
        {
            var hour = DateTime.Now.Hour;
            Greeting = hour switch
            {
                >= 5 and < 12 => "Good morning, Alex!",
                >= 12 and < 17 => "Good afternoon, Alex!",
                >= 17 and < 22 => "Good evening, Alex!",
                _ => "Good night, Alex!"
            };
        }

        private void LoadDashboardData()
        {
            // Load task statistics
            var allTasks = _taskRepository.GetAllTasks();
            TotalTasks = allTasks.Count;
            CompletedTasks = _taskRepository.GetCompletedTasksCount();
            Streak = _taskRepository.GetCompletionStreak();

            // Load today's tasks and sort by priority
            var todaysTasks = _taskRepository.GetTasksDueToday()
                .OrderBy(t => t.IsCompleted)  // Uncompleted tasks first
                .ThenByDescending(t => t.Priority) // High priority first
                .ToList();
            TodaysTasks = new ObservableCollection<TodoTask>(todaysTasks);

            // Load diary statistics
            var allEntries = _diaryRepository.GetAllEntries();
            DiaryEntries = allEntries.Count;

            // Load recent diary entries (last 5)
            var recentEntries = allEntries.Take(5).ToList();
            RecentDiaryEntries = new ObservableCollection<DiaryEntry>(recentEntries);
        }

        private void ToggleTaskCompletion(TodoTask task)
        {
            System.Diagnostics.Debug.WriteLine($"Toggle task called for task: {task.Id} - {task.Title}");
            try 
            {
                // Update UI state immediately
                task.IsCompleted = !task.IsCompleted;
                
                // Update database
                _taskRepository.ToggleTaskCompletion(task);
                System.Diagnostics.Debug.WriteLine("Repository ToggleTaskCompletion called");
                
                // Refresh data
                LoadDashboardData();
                System.Diagnostics.Debug.WriteLine("Data reloaded");
            }
            catch (Exception ex)
            {
                // Revert UI state if there was an error
                task.IsCompleted = !task.IsCompleted;
                System.Diagnostics.Debug.WriteLine($"Error in ToggleTaskCompletion: {ex.Message}");
            }
        }

        private void NavigateToTask(TodoTask task)
        {
            // TODO: Implement navigation to task detail/edit view
            // This will be implemented when we add navigation service
        }

        private void NavigateToDiary(DiaryEntry entry)
        {
            // TODO: Implement navigation to diary entry detail view
            // This will be implemented when we add navigation service
        }

        private void DeleteTask(TodoTask task)
        {
            _taskRepository.DeleteTask(task);
            LoadDashboardData(); // Refresh data after deletion
        }

        private void SetRandomQuote()
        {
            var quotes = new[]
            {
                "\"First, solve the problem. Then, write the code.\"",
                "\"Programming isn't about what you know; it's about what you can figure out.\"",
                "\"The best error message is the one that never shows up.\"",
                "\"Make it work, make it right, make it fast.\"",
                "\"Every great developer you know got there by solving problems they were unqualified to solve until they actually did it.\"",
                "\"The only way to learn a new programming language is by writing programs in it.\"",
                "\"Sometimes it pays to stay in bed on Monday, rather than spending the rest of the week debugging Monday's code.\"",
                "\"Code is like humor. When you have to explain it, it's bad.\"",
                "\"Simplicity is the soul of efficiency.\"",
                "\"Before software can be reusable it first has to be usable.\"",
                "\"The best way to predict the future is to implement it.\"",
                "\"Any fool can write code that a computer can understand. Good programmers write code that humans can understand.\"",
                "\"Experience is the name everyone gives to their mistakes.\"",
                "\"The most damaging phrase in the language is 'We've always done it this way.'\"",
                "\"The function of good software is to make the complex appear to be simple.\"",
                "\"Don't comment bad code - rewrite it.\"",
                "\"The best preparation for tomorrow is doing your best today.\"",
                "\"The only way to do great work is to love what you do.\"",
                "\"Quality is not an act, it is a habit.\"",
                "\"Perfection is achieved not when there is nothing more to add, but rather when there is nothing more to take away.\""
            };

            var random = new Random();
            Quote = quotes[random.Next(quotes.Length)];
        }

        public void Dispose()
        {
            _timer?.Stop();
            _taskRepository?.Dispose();
            _diaryRepository?.Dispose();
        }
    }
} 