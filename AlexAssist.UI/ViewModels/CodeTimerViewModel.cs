using System;
using System.Windows.Input;
using System.Windows.Threading;
using AlexAssist.UI.Models;

namespace AlexAssist.UI.ViewModels
{
    public class CodeTimerViewModel : BaseViewModel, IDisposable
    {
        private readonly CodingSessionRepository _repository;
        private readonly DispatcherTimer _timer;
        private CodingSession? _activeSession;
        private string _currentTime = "00:00:00";
        private string _totalTimeToday = "00:00:00";
        private string _projectName = string.Empty;
        private string _description = string.Empty;
        private bool _isRunning;

        public string CurrentTime
        {
            get => _currentTime;
            set => SetField(ref _currentTime, value);
        }

        public string TotalTimeToday
        {
            get => _totalTimeToday;
            set => SetField(ref _totalTimeToday, value);
        }

        public string ProjectName
        {
            get => _projectName;
            set => SetField(ref _projectName, value);
        }

        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetField(ref _isRunning, value);
        }

        public ICommand StartTimerCommand { get; }
        public ICommand StopTimerCommand { get; }

        public CodeTimerViewModel()
        {
            _repository = new CodingSessionRepository();
            
            // Initialize timer
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            // Initialize commands
            StartTimerCommand = new RelayCommand(_ => StartTimer(), _ => !IsRunning);
            StopTimerCommand = new RelayCommand(_ => StopTimer(), _ => IsRunning);

            // Check for active session
            LoadActiveSession();
            UpdateTotalTimeToday();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_activeSession != null)
            {
                CurrentTime = _activeSession.Duration.ToString(@"hh\:mm\:ss");
            }
            UpdateTotalTimeToday();
        }

        private void StartTimer()
        {
            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                ProjectName = "Untitled Project";
            }

            _activeSession = new CodingSession
            {
                StartTime = DateTime.Now,
                Project = ProjectName,
                Description = Description
            };

            _repository.StartSession(_activeSession);
            _timer.Start();
            IsRunning = true;
        }

        private void StopTimer()
        {
            if (_activeSession != null)
            {
                _repository.EndSession(_activeSession);
                _activeSession = null;
            }

            _timer.Stop();
            IsRunning = false;
            CurrentTime = "00:00:00";
            ProjectName = string.Empty;
            Description = string.Empty;
        }

        private void LoadActiveSession()
        {
            _activeSession = _repository.GetActiveSession();
            if (_activeSession != null)
            {
                ProjectName = _activeSession.Project;
                Description = _activeSession.Description;
                IsRunning = true;
                _timer.Start();
            }
        }

        private void UpdateTotalTimeToday()
        {
            var totalTime = _repository.GetTotalTimeToday();
            TotalTimeToday = totalTime.ToString(@"hh\:mm\:ss");
        }

        public void Dispose()
        {
            _timer.Stop();
            _repository.Dispose();
        }
    }
} 