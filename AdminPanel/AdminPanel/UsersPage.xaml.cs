using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http.Json;
using System.Text.Json;
using System.Collections.Generic;

namespace AdminPanel
{
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            try
            {
                var response = await ApiClient.httpClient.GetAsync("users/fetchUsers");

                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<User>>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    UsersListView.ItemsSource = users;
                }
                else
                {
                    MessageBox.Show($"Error fetching users: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Dashboard());
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var user = button.DataContext as User;
                if (user != null)
                {
                    // Optional: Check if the current user is logged in (adjust as needed)
                    if (CurrentUser.UserId == 0) // Assuming CurrentUser is a static class
                    {
                        MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var result = MessageBox.Show("Are you sure you want to delete this user?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        string deleteUrl = $"users/deleteUser?UserId={user.UserID}";
                        try
                        {
                            var response = await ApiClient.httpClient.DeleteAsync(deleteUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                LoadUsers(); // Refresh the list
                                MessageBox.Show("User deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                MessageBox.Show("You do not have permission to delete this user.", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                MessageBox.Show($"Failed to delete user: {response.StatusCode}\n{errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No user selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            }
        private void UsersListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gridView = UsersListView.View as GridView;
            if (gridView != null)
            {
                double totalWidth = UsersListView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
                double availableWidth = totalWidth - 20; 

                double[] proportions = { 0.05, 0.15, 0.15, 0.25, 0.1, 0.1, 0.05, 0.15 };
                for (int i = 0; i < gridView.Columns.Count; i++)
                {
                    gridView.Columns[i].Width = availableWidth * proportions[i];
                }
            }
        }
    }
}