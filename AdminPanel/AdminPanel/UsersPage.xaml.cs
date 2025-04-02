using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel
{
    public class DeleteUserRequestDto
    {
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
            if (sender is Button button && button.DataContext is User user)
            {
                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete {user.FirstName} {user.LastName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (confirmResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        var deleteRequest = new DeleteUserRequestDto
                        {
                            Password = "",
                            FirstName = user.FirstName,
                            LastName = user.LastName
                        };

                        string deleteUrl = $"users/deleteUser?userId={user.UserId}&FirstName={Uri.EscapeDataString(user.FirstName)}&LastName={Uri.EscapeDataString(user.LastName)}";

                        var request = new HttpRequestMessage(HttpMethod.Delete, deleteUrl)
                        {
                            Content = JsonContent.Create(deleteRequest)
                        };

                        var response = await ApiClient.httpClient.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            LoadUsers();
                            MessageBox.Show("User deleted successfully!",
                                "Success",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Failed to delete user: {errorContent}",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
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

                double[] proportions = { 0.08, 0.20, 0.20, 0.15, 0.20, 0.10, 0.07 };
                for (int i = 0; i < gridView.Columns.Count; i++)
                {
                    gridView.Columns[i].Width = availableWidth * proportions[i];
                }
            }
        }
    }
}