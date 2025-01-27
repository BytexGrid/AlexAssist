using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using AlexAssist.UI.Models;
using AlexAssist.UI.Views.Diary;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;

namespace AlexAssist.UI.ViewModels
{
    public class DiaryViewModel : BaseViewModel, IDisposable
    {
        private readonly DiaryRepository _repository;
        private ObservableCollection<DiaryEntry> _entries = new();
        private ObservableCollection<DiaryEntry> _filteredEntries = new();
        private DiaryEntry? _selectedEntry;
        private string _searchText = string.Empty;
        private string _newEntryTitle = string.Empty;
        private string _newEntryContent = string.Empty;
        private string _selectedMood = string.Empty;
        private DateTime _selectedDate = DateTime.Now;
        private bool _isEditing;
        private ObservableCollection<CodeSnippet> _codeSnippets = new();
        private bool _hasCodeSnippets;

        public ObservableCollection<DiaryEntry> Entries
        {
            get => _entries;
            set => SetField(ref _entries, value);
        }

        public ObservableCollection<DiaryEntry> FilteredEntries
        {
            get => _filteredEntries;
            set => SetField(ref _filteredEntries, value);
        }

        public DiaryEntry? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (SetField(ref _selectedEntry, value))
                {
                    // Update editor fields when entry is selected
                    if (value != null)
                    {
                        NewEntryTitle = value.Title;
                        NewEntryContent = value.Content;
                        SelectedMood = value.Mood;
                        SelectedDate = value.CreatedAt;
                        IsEditing = true;
                        CodeSnippets = new ObservableCollection<CodeSnippet>(value.CodeSnippets);
                        HasCodeSnippets = value.CodeSnippets.Any();
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
                    FilterEntries();
                }
            }
        }

        public string NewEntryTitle
        {
            get => _newEntryTitle;
            set => SetField(ref _newEntryTitle, value);
        }

        public string NewEntryContent
        {
            get => _newEntryContent;
            set => SetField(ref _newEntryContent, value);
        }

        public string SelectedMood
        {
            get => _selectedMood;
            set => SetField(ref _selectedMood, value);
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetField(ref _selectedDate, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetField(ref _isEditing, value);
        }

        public ObservableCollection<string> AvailableMoods { get; } = new()
        {
            "😊 Happy",
            "😌 Calm",
            "🤔 Thoughtful",
            "😤 Frustrated",
            "😢 Sad",
            "😴 Tired",
            "🎯 Focused",
            "💪 Motivated"
        };

        public ObservableCollection<CodeSnippet> CodeSnippets
        {
            get => _codeSnippets;
            set => SetField(ref _codeSnippets, value);
        }

        public bool HasCodeSnippets
        {
            get => _hasCodeSnippets;
            set => SetField(ref _hasCodeSnippets, value);
        }

        public ICommand NewEntryCommand { get; }
        public ICommand SaveEntryCommand { get; }
        public ICommand DeleteEntryCommand { get; }
        public ICommand ClearEditorCommand { get; }
        public ICommand AddCodeSnippetCommand { get; }
        public ICommand RemoveCodeSnippetCommand { get; }

        public DiaryViewModel()
        {
            _repository = new DiaryRepository();
            
            // Initialize commands
            NewEntryCommand = new RelayCommand(_ => NewEntry());
            SaveEntryCommand = new RelayCommand(_ => SaveEntry(), _ => CanSaveEntry());
            DeleteEntryCommand = new RelayCommand(_ => DeleteEntry(), _ => SelectedEntry != null);
            ClearEditorCommand = new RelayCommand(_ => ClearEditor());
            AddCodeSnippetCommand = new RelayCommand(async _ => await ShowAddCodeSnippetDialog());
            RemoveCodeSnippetCommand = new RelayCommand(snippet => RemoveCodeSnippet((CodeSnippet)snippet));

            // Load data from database
            LoadEntries();
            FilterEntries(); // Initialize filtered entries
        }

        private void LoadEntries()
        {
            try
            {
                var entries = _repository.GetAllEntries();
                Entries = new ObservableCollection<DiaryEntry>(entries);
            }
            catch
            {
                // If loading fails, start with empty collection
                Entries = new ObservableCollection<DiaryEntry>();
            }
        }

        private void NewEntry()
        {
            SelectedEntry = null;
            ClearEditor();
            IsEditing = false;
        }

        private void FilterEntries()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredEntries = new ObservableCollection<DiaryEntry>(Entries);
                return;
            }

            var searchTerms = SearchText.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var filtered = Entries.Where(entry =>
                searchTerms.All(term =>
                    entry.Title.ToLower().Contains(term) ||
                    entry.Content.ToLower().Contains(term) ||
                    entry.Mood.ToLower().Contains(term) ||
                    entry.CreatedAt.ToString("MMM dd yyyy").ToLower().Contains(term)
                )
            ).ToList();

            FilteredEntries = new ObservableCollection<DiaryEntry>(filtered);
        }

        private void UpdateSnippetsTab()
        {
            HasCodeSnippets = CodeSnippets.Any();
        }

        private async Task ShowAddCodeSnippetDialog()
        {
            var dialog = new CodeSnippetDialog();
            var result = await DialogHost.Show(dialog, "RootDialog");

            if (result is CodeSnippet snippet)
            {
                CodeSnippets.Add(snippet);
                UpdateSnippetsTab();
            }
        }

        private void RemoveCodeSnippet(CodeSnippet snippet)
        {
            CodeSnippets.Remove(snippet);
            UpdateSnippetsTab();
        }

        private void SaveEntry()
        {
            if (IsEditing && SelectedEntry != null)
            {
                // Update existing entry
                var index = Entries.IndexOf(SelectedEntry);
                SelectedEntry.Title = NewEntryTitle;
                SelectedEntry.Content = NewEntryContent;
                SelectedEntry.Mood = SelectedMood;
                SelectedEntry.CreatedAt = SelectedDate;
                SelectedEntry.CodeSnippets = new List<CodeSnippet>(CodeSnippets);
                
                try
                {
                    _repository.UpdateEntry(SelectedEntry);
                    // Force UI update by removing and re-adding
                    Entries.RemoveAt(index);
                    Entries.Insert(index, SelectedEntry);
                }
                catch
                {
                    // If update fails, keep UI state as is
                }
            }
            else
            {
                // Create new entry
                var entry = new DiaryEntry
                {
                    Title = NewEntryTitle,
                    Content = NewEntryContent,
                    Mood = SelectedMood,
                    CreatedAt = SelectedDate,
                    CodeSnippets = new List<CodeSnippet>(CodeSnippets)
                };

                try
                {
                    _repository.AddEntry(entry);
                    Entries.Add(entry);
                }
                catch
                {
                    // If save fails, don't add to UI
                }
            }

            // Update filtered entries
            FilterEntries();
            ClearEditor();
        }

        private void DeleteEntry()
        {
            if (SelectedEntry != null)
            {
                try
                {
                    _repository.DeleteEntry(SelectedEntry);
                    Entries.Remove(SelectedEntry);
                    FilterEntries(); // Update filtered entries
                }
                catch
                {
                    // If delete fails, keep UI state as is
                }
                ClearEditor();
            }
        }

        private void ClearEditor()
        {
            NewEntryTitle = string.Empty;
            NewEntryContent = string.Empty;
            SelectedMood = string.Empty;
            SelectedDate = DateTime.Now;
            IsEditing = false;
            SelectedEntry = null;
            CodeSnippets.Clear();
            UpdateSnippetsTab();
        }

        private bool CanSaveEntry()
        {
            return !string.IsNullOrWhiteSpace(NewEntryTitle) && 
                   !string.IsNullOrWhiteSpace(NewEntryContent);
        }

        public void LoadEntry(DiaryEntry entry)
        {
            NewEntryTitle = entry.Title;
            NewEntryContent = entry.Content;
            SelectedMood = entry.Mood;
            SelectedDate = entry.CreatedAt;
            CodeSnippets = new ObservableCollection<CodeSnippet>(entry.CodeSnippets);
            HasCodeSnippets = entry.CodeSnippets.Any();
            IsEditing = true;
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool>? _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter!) ?? true;

        public void Execute(object? parameter) => _execute(parameter!);
    }
} 