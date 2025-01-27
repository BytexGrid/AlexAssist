using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using AlexAssist.UI.Views.Dashboard;
using System.IO;
using System.Text.Json;

namespace AlexAssist.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private bool _isDarkTheme;
        private object? _currentView;
        private readonly PaletteHelper _paletteHelper = new();

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (SetField(ref _isDarkTheme, value))
                {
                    ModifyTheme(value);
                    SaveThemePreference(value);
                }
            }
        }

        public object? CurrentView
        {
            get => _currentView;
            set => SetField(ref _currentView, value);
        }

        private void ModifyTheme(bool isDarkTheme)
        {
            var theme = _paletteHelper.GetTheme();
            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            _paletteHelper.SetTheme(theme);
        }

        public MainViewModel()
        {
            // Load theme preference at startup
            LoadThemePreference();
            
            // Show dashboard by default
            CurrentView = new DashboardView();
        }

        private void LoadThemePreference()
        {
            try
            {
                var preferencesPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "AlexAssist",
                    "preferences.json"
                );

                if (File.Exists(preferencesPath))
                {
                    var jsonString = File.ReadAllText(preferencesPath);
                    var preferences = JsonSerializer.Deserialize<dynamic>(jsonString);
                    IsDarkTheme = preferences.GetProperty("IsDarkTheme").GetBoolean();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading theme preference: {ex.Message}");
            }
        }

        private void SaveThemePreference(bool isDarkTheme)
        {
            try
            {
                var appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "AlexAssist"
                );
                Directory.CreateDirectory(appDataPath);

                var preferencesPath = Path.Combine(appDataPath, "preferences.json");
                var preferences = new { IsDarkTheme = isDarkTheme };
                var jsonString = JsonSerializer.Serialize(preferences);
                File.WriteAllText(preferencesPath, jsonString);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving theme preference: {ex.Message}");
            }
        }
    }
} 