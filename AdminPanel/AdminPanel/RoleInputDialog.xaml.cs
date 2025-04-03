using System.Windows;
using System.Windows.Controls;

namespace AdminPanel
{
    public partial class RoleInputDialog : Window
    {
        public string Role { get; set; }

        public RoleInputDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (RoleComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                Role = selectedItem.Content.ToString();
                DialogResult = true;
            }
        }
    }
}