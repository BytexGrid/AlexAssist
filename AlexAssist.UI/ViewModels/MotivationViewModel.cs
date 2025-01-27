using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using AlexAssist.UI.Models;

namespace AlexAssist.UI.ViewModels
{
    public class MotivationViewModel : BaseViewModel, IDisposable
    {
        private readonly QuoteRepository _quoteRepository;
        private readonly TaskRepository _taskRepository;
        private Quote? _currentQuote;
        private ObservableCollection<Quote> _favoriteQuotes = new();
        private string _selectedCategory = "All";

        public Quote? CurrentQuote
        {
            get => _currentQuote;
            set => SetField(ref _currentQuote, value);
        }

        public ObservableCollection<Quote> FavoriteQuotes
        {
            get => _favoriteQuotes;
            set => SetField(ref _favoriteQuotes, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetField(ref _selectedCategory, value))
                {
                    LoadQuotesByCategory();
                }
            }
        }

        public ObservableCollection<string> Categories { get; } = new()
        {
            "All",
            "Success",
            "Growth",
            "Productivity",
            "Inspiration",
            "Focus"
        };

        // Achievement Properties
        public int CompletedTasksCount => _taskRepository.GetCompletedTasksCount();
        public int TotalTasksCount => _taskRepository.GetAllTasks().Count;
        public int TaskCompletionStreak => _taskRepository.GetCompletionStreak();
        public double TaskCompletionRate => TotalTasksCount > 0 ? (double)CompletedTasksCount / TotalTasksCount * 100 : 0;

        public ICommand NewQuoteCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }

        public MotivationViewModel()
        {
            _quoteRepository = new QuoteRepository();
            _taskRepository = new TaskRepository();

            NewQuoteCommand = new RelayCommand(_ => LoadRandomQuote());
            ToggleFavoriteCommand = new RelayCommand(_ => ToggleCurrentQuoteFavorite());

            LoadRandomQuote();
            LoadFavorites();
        }

        private void LoadRandomQuote()
        {
            if (_selectedCategory == "All")
            {
                CurrentQuote = _quoteRepository.GetRandomQuote();
            }
            else
            {
                var categoryQuotes = _quoteRepository.GetQuotesByCategory(_selectedCategory);
                if (categoryQuotes.Any())
                {
                    var random = new Random();
                    CurrentQuote = categoryQuotes[random.Next(categoryQuotes.Count)];
                }
            }
        }

        private void LoadQuotesByCategory()
        {
            LoadRandomQuote();
        }

        private void LoadFavorites()
        {
            var favorites = _quoteRepository.GetFavorites();
            FavoriteQuotes = new ObservableCollection<Quote>(favorites);
        }

        private void ToggleCurrentQuoteFavorite()
        {
            if (CurrentQuote != null)
            {
                _quoteRepository.ToggleFavorite(CurrentQuote.Id);
                CurrentQuote.IsFavorite = !CurrentQuote.IsFavorite;
                LoadFavorites();
            }
        }

        public void Dispose()
        {
            _quoteRepository?.Dispose();
            _taskRepository?.Dispose();
        }
    }
} 