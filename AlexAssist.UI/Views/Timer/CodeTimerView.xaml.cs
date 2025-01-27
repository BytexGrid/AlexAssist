using System.Windows.Controls;
using AlexAssist.UI.ViewModels;

namespace AlexAssist.UI.Views.Timer
{
    public partial class CodeTimerView : UserControl
    {
        public CodeTimerView()
        {
            InitializeComponent();
            DataContext = new CodeTimerViewModel();
        }
    }
} 