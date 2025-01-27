using System.Windows;
using System.Windows.Controls;
using AlexAssist.UI.Models;
using MaterialDesignThemes.Wpf;

namespace AlexAssist.UI.Views.Diary
{
    public partial class CodeSnippetDialog : UserControl
    {
        public CodeSnippetDialog()
        {
            InitializeComponent();
            LanguageComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CodeTextBox.Text))
            {
                return;
            }

            var snippet = new CodeSnippet
            {
                Language = ((ComboBoxItem)LanguageComboBox.SelectedItem).Content.ToString() ?? "Other",
                Description = DescriptionTextBox.Text,
                Code = CodeTextBox.Text
            };

            DialogHost.CloseDialogCommand.Execute(snippet, null);
        }
    }
} 