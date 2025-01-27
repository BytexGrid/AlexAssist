using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Text.Json;
using Microsoft.Win32;
using AlexAssist.UI.Models;

namespace AlexAssist.UI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly TaskRepository _taskRepository;
        private readonly DiaryRepository _diaryRepository;
        private readonly MainViewModel _mainViewModel;

        public bool IsDarkTheme
        {
            get => _mainViewModel.IsDarkTheme;
            set => _mainViewModel.IsDarkTheme = value;
        }

        public ICommand ExportDatabaseCommand { get; }
        public ICommand ImportDatabaseCommand { get; }

        public SettingsViewModel()
        {
            _taskRepository = new TaskRepository();
            _diaryRepository = new DiaryRepository();
            
            var mainWindow = App.Current.MainWindow;
            if (mainWindow?.DataContext is not MainViewModel mainViewModel)
            {
                throw new InvalidOperationException("MainViewModel not found in MainWindow.DataContext");
            }
            _mainViewModel = mainViewModel;

            ExportDatabaseCommand = new RelayCommand(_ => ExportDatabase());
            ImportDatabaseCommand = new RelayCommand(_ => ImportDatabase());
        }

        private void ExportDatabase()
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    DefaultExt = ".json",
                    FileName = $"alexassist_backup_{DateTime.Now:yyyyMMdd}"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    var exportData = new
                    {
                        Tasks = _taskRepository.GetAllTasks(),
                        DiaryEntries = _diaryRepository.GetAllEntries(),
                        Theme = IsDarkTheme
                    };

                    var jsonString = JsonSerializer.Serialize(exportData, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });
                    File.WriteAllText(saveDialog.FileName, jsonString);
                }
            }
            catch (Exception ex)
            {
                // TODO: Show error dialog
                System.Diagnostics.Debug.WriteLine($"Export error: {ex.Message}");
            }
        }

        private void ImportDatabase()
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json",
                    DefaultExt = ".json"
                };

                if (openDialog.ShowDialog() == true)
                {
                    var jsonString = File.ReadAllText(openDialog.FileName);
                    var importData = JsonSerializer.Deserialize<dynamic>(jsonString);

                    // TODO: Implement import logic
                    // This will require careful handling of existing data
                    // Consider adding a confirmation dialog
                    // Consider handling conflicts
                }
            }
            catch (Exception ex)
            {
                // TODO: Show error dialog
                System.Diagnostics.Debug.WriteLine($"Import error: {ex.Message}");
            }
        }
    }
} 