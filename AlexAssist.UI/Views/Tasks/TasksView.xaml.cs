using System.Windows.Controls;
using AlexAssist.UI.ViewModels;

namespace AlexAssist.UI.Views.Tasks
{
    public partial class TasksView : UserControl
    {
        public TasksView()
        {
            InitializeComponent();
            DataContext = new TaskViewModel();
        }
    }
} 