using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        // Update placeholder visibility based on password content.
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility =
                string.IsNullOrEmpty(Password.Password) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Keep plain text and PasswordBox in sync.
        private void PlainPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (plainPassword.Visibility == Visibility.Visible)
            {
                Password.Password = plainPassword.Text;
            }
        }

        // Toggle: Show plain text password.
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            plainPassword.Text = Password.Password;
            Password.Visibility = Visibility.Collapsed;
            plainPassword.Visibility = Visibility.Visible;
        }

        // Toggle: Hide plain text password.
        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            Password.Password = plainPassword.Text;
            plainPassword.Visibility = Visibility.Collapsed;
            Password.Visibility = Visibility.Visible;
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous inline message.
            InlineMessage.Text = "";

            string email = userName.Text;
            // Use whichever control is visible for password.
            string password = Password.Visibility == Visibility.Visible ? Password.Password : plainPassword.Text;

            try
            {
                // Show loading indicator
                LoadingIndicator.Visibility = Visibility.Visible;

                // Replace this with your actual login async call.
                string result = await LoginUsers.LoginAsync(email, password);

                // Hide loading indicator
                LoadingIndicator.Visibility = Visibility.Collapsed;

                if (result == "Login successful")
                {
                    CurrentUser.UserName = userName.Text;  
                    InlineMessage.Foreground = new SolidColorBrush(Colors.LightGreen);
                    InlineMessage.Text = "Login successful.";
                    await System.Threading.Tasks.Task.Delay(1000);
                    NavigationService.Navigate(new Dashboard());
                }
                else
                {
                    InlineMessage.Foreground = new SolidColorBrush(Colors.OrangeRed);
                    InlineMessage.Text = result;
                }
            }
            catch (Exception ex)
            {
                LoadingIndicator.Visibility = Visibility.Collapsed;
                InlineMessage.Foreground = new SolidColorBrush(Colors.OrangeRed);
                InlineMessage.Text = $"An unexpected error occurred: {ex.Message}";
            }
        }
        
    }
}
