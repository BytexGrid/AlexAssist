using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using AlexAssist.UI.Models;
using MaterialDesignThemes.Wpf;

namespace AlexAssist.UI.ViewModels
{
    public class TaskViewModel : BaseViewModel, IDisposable
    {
        private readonly TaskRepository _repository;
        private ObservableCollection<TodoTask> _tasks = new();
        private ObservableCollection<TodoTask> _filteredTasks = new();
        private TodoTask? _selectedTask;
        private string _searchText = string.Empty;
        private string _newTaskTitle = string.Empty;
        private string _newTaskDescription = string.Empty;
        private DateTime? _selectedDueDate;
        private string _selectedPriority = "Medium";
        private string _selectedCategory = "General";
        private bool _isEditing;
        private bool _showCompletedTasks = true;
        private bool _isTaskEditorVisible;

        public bool IsTaskEditorVisible
        {
            get => _isTaskEditorVisible;
            set => SetField(ref _isTaskEditorVisible, value);
        }

        public ObservableCollection<TodoTask> Tasks
        {
            get => _tasks;
            set => SetField(ref _tasks, value);
        }

        public ObservableCollection<TodoTask> FilteredTasks
        {
            get => _filteredTasks;
            set => SetField(ref _filteredTasks, value);
        }

        public TodoTask? SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (SetField(ref _selectedTask, value))
                {
                    if (value != null)
                    {
                        NewTaskTitle = value.Title;
                        NewTaskDescription = value.Description;
                        SelectedDueDate = value.DueDate;
                        SelectedPriority = value.Priority;
                        SelectedCategory = value.Category;
                        IsEditing = true;
                    }
                    else
                    {
                        ClearEditor();
                    }
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetField(ref _searchText, value))
                {
                    FilterTasks();
                }
            }
        }

        public string NewTaskTitle
        {
            get => _newTaskTitle;
            set => SetField(ref _newTaskTitle, value);
        }

        public string NewTaskDescription
        {
            get => _newTaskDescription;
            set => SetField(ref _newTaskDescription, value);
        }

        public DateTime? SelectedDueDate
        {
            get => _selectedDueDate;
            set => SetField(ref _selectedDueDate, value);
        }

        public string SelectedPriority
        {
            get => _selectedPriority;
            set => SetField(ref _selectedPriority, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetField(ref _selectedCategory, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetField(ref _isEditing, value);
        }

        public bool ShowCompletedTasks
        {
            get => _showCompletedTasks;
            set
            {
                if (SetField(ref _showCompletedTasks, value))
                {
                    FilterTasks();
                }
            }
        }

        public ObservableCollection<string> AvailablePriorities { get; } = new()
        {
            "High",
            "Medium",
            "Low"
        };

        public ObservableCollection<string> AvailableCategories { get; } = new()
        {
            "General",
            "Work",
            "Personal",
            "Shopping",
            "Health",
            "Learning"
        };

        public ICommand NewTaskCommand { get; }
        public ICommand SaveTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand ClearEditorCommand { get; }
        public ICommand ToggleCompletionCommand { get; }
        public ICommand CloseEditorCommand { get; }

        public TaskViewModel()
        {
            _repository = new TaskRepository();

            // Initialize commands
            NewTaskCommand = new RelayCommand(_ => NewTask());
            SaveTaskCommand = new RelayCommand(_ => SaveTask(), _ => CanSaveTask());
            DeleteTaskCommand = new RelayCommand(task => DeleteTask((TodoTask)task));
            EditTaskCommand = new RelayCommand(task => EditTask((TodoTask)task));
            ClearEditorCommand = new RelayCommand(_ => ClearEditor());
            ToggleCompletionCommand = new RelayCommand(task => ToggleTaskCompletion((TodoTask)task));
            CloseEditorCommand = new RelayCommand(_ => CloseEditor());

            // Load tasks from database
            LoadTasks();
            FilterTasks();
        }

        private void LoadTasks()
        {
            try
            {
                var tasks = _repository.GetAllTasks();
                Tasks = new ObservableCollection<TodoTask>(tasks);
            }
            catch
            {
                Tasks = new ObservableCollection<TodoTask>();
            }
        }

        private void FilterTasks()
        {
            var filtered = Tasks.AsEnumerable();

            // Filter by completion status
            if (!ShowCompletedTasks)
            {
                filtered = filtered.Where(t => !t.IsCompleted);
            }

            // Filter by search text
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchTerms = SearchText.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                filtered = filtered.Where(task =>
                    searchTerms.All(term =>
                        task.Title.ToLower().Contains(term) ||
                        task.Description.ToLower().Contains(term) ||
                        task.Category.ToLower().Contains(term) ||
                        task.Priority.ToLower().Contains(term)
                    )
                );
            }

            FilteredTasks = new ObservableCollection<TodoTask>(filtered.OrderByDescending(t => t.DueDate));
        }

        private void NewTask()
        {
            SelectedTask = null;
            ClearEditor();
            IsEditing = false;
            IsTaskEditorVisible = true;
        }

        private void CloseEditor()
        {
            ClearEditor();
            IsTaskEditorVisible = false;
        }

        private void SaveTask()
        {
            if (IsEditing && SelectedTask != null)
            {
                // Update existing task
                SelectedTask.Title = NewTaskTitle;
                SelectedTask.Description = NewTaskDescription;
                SelectedTask.DueDate = SelectedDueDate;
                SelectedTask.Priority = SelectedPriority;
                SelectedTask.Category = SelectedCategory;

                try
                {
                    _repository.UpdateTask(SelectedTask);
                    var index = Tasks.IndexOf(SelectedTask);
                    Tasks.RemoveAt(index);
                    Tasks.Insert(index, SelectedTask);
                }
                catch
                {
                    // Handle error
                }
            }
            else
            {
                // Create new task
                var task = new TodoTask
                {
                    Title = NewTaskTitle,
                    Description = NewTaskDescription,
                    DueDate = SelectedDueDate,
                    Priority = SelectedPriority,
                    Category = SelectedCategory,
                    CreatedAt = DateTime.Now
                };

                try
                {
                    _repository.AddTask(task);
                    Tasks.Add(task);
                }
                catch
                {
                    // Handle error
                }
            }

            FilterTasks();
            CloseEditor();
        }

        private void EditTask(TodoTask task)
        {
            SelectedTask = task;
            IsTaskEditorVisible = true;
        }

        private void DeleteTask(TodoTask task)
        {
            try
            {
                _repository.DeleteTask(task);
                Tasks.Remove(task);
                FilterTasks();
            }
            catch
            {
                // Handle error
            }
            ClearEditor();
        }

        private void ToggleTaskCompletion(TodoTask task)
        {
            try
            {
                // Update UI state immediately
                task.IsCompleted = !task.IsCompleted;
                
                // Update database
                _repository.ToggleTaskCompletion(task);
                
                // Refresh filtered view
                FilterTasks();
            }
            catch (Exception ex)
            {
                // Revert UI state if there was an error
                task.IsCompleted = !task.IsCompleted;
                System.Diagnostics.Debug.WriteLine($"Error in ToggleTaskCompletion: {ex.Message}");
            }
        }

        private void ClearEditor()
        {
            NewTaskTitle = string.Empty;
            NewTaskDescription = string.Empty;
            SelectedDueDate = null;
            SelectedPriority = "Medium";
            SelectedCategory = "General";
            IsEditing = false;
            SelectedTask = null;
        }

        private bool CanSaveTask()
        {
            return !string.IsNullOrWhiteSpace(NewTaskTitle);
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
} 