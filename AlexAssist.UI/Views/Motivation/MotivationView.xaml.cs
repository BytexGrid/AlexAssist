using System.Windows.Controls;
using AlexAssist.UI.ViewModels;

namespace AlexAssist.UI.Views.Motivation
{
    public partial class MotivationView : UserControl
    {
        public MotivationView()
        {
            InitializeComponent();
            DataContext = new MotivationViewModel();
        }
    }
} 