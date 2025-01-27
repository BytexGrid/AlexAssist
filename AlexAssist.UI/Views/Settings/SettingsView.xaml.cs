using System.Windows.Controls;
using AlexAssist.UI.ViewModels;

namespace AlexAssist.UI.Views.Settings
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
            DataContext = new SettingsViewModel();
        }
    }
} 