using System.Windows.Controls;
using AlexAssist.UI.ViewModels;

namespace AlexAssist.UI.Views.Diary
{
    public partial class DiaryView : UserControl
    {
        public DiaryView()
        {
            InitializeComponent();
            DataContext = new DiaryViewModel();
        }
    }
} 