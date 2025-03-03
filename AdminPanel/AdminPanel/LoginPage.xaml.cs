using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdminPanel
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            
            PasswordPlaceholder.Visibility =
                (string.IsNullOrEmpty(Password.Password)) ? Visibility.Visible : Visibility.Collapsed;
        }
        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            string email = userName.Text;
            string password = Password.Password;

            
            string result = await LoginUsers.LoginAsync(email, password);

            if (result == "Login successful")
            {
                
                MessageBox.Show("Login successful", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                
                NavigationService.Navigate(new Dashboard());
            }
            else
            {
                
                MessageBox.Show(result, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
