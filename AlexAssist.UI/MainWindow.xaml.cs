using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using AlexAssist.UI.ViewModels;
using AlexAssist.UI.Views.Dashboard;
using AlexAssist.UI.Views.Diary;
using AlexAssist.UI.Views.Tasks;
using AlexAssist.UI.Views.Motivation;
using AlexAssist.UI.Views.Settings;

namespace AlexAssist.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly PaletteHelper _paletteHelper = new();
    private readonly MainViewModel ViewModel;

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainViewModel();
        DataContext = ViewModel;
        MenuItems.SelectionChanged += MenuItems_SelectionChanged;
        
        // Show dashboard by default
        ViewModel.CurrentView = new DashboardView();
    }

    private void MenuItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = MenuItems.SelectedIndex;
        switch (index)
        {
            case 0: // Dashboard
                ViewModel.CurrentView = new DashboardView();
                break;
            case 1: // Diary
                ViewModel.CurrentView = new DiaryView();
                break;
            case 2: // Tasks
                ViewModel.CurrentView = new TasksView();
                break;
            case 3: // Motivation
                ViewModel.CurrentView = new MotivationView();
                break;
            case 4: // Settings
                ViewModel.CurrentView = new SettingsView();
                break;
        }
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsDarkTheme = !ViewModel.IsDarkTheme;
    }

    private void ModifyTheme(bool isDarkTheme)
    {
        var theme = _paletteHelper.GetTheme();
        theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
        _paletteHelper.SetTheme(theme);
    }
}