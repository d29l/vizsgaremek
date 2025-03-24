using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel
{
    public partial class PostsPage : Page
    {
        public PostsPage()
        {
            InitializeComponent();
            LoadPosts();
        }

        private async void LoadPosts()
        {
            try
            {
                var response = await ApiClient.httpClient.GetAsync("posts/fetchPosts");
                if (response.IsSuccessStatusCode)
                {
                    var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
                    PostsListView.ItemsSource = posts;
                }
                else
                {
                    MessageBox.Show("Failed to load posts.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var post = button.DataContext as Post;
                if (post != null)
                {
                    if (CurrentUser.UserId == 0)
                    {
                        MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var result = MessageBox.Show("Are you sure you want to delete this post?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        int userId = CurrentUser.UserId;
                        string deleteUrl = $"posts/deletePost?PostId={post.PostId}&EmployerId={post.EmployerId}&userId={userId}";
                        try
                        {
                            var response = await ApiClient.httpClient.DeleteAsync(deleteUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                LoadPosts();
                                MessageBox.Show("Post deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            {
                                MessageBox.Show("You do not have permission to delete this post.", "Permission Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                var errorContent = await response.Content.ReadAsStringAsync();
                                MessageBox.Show($"Failed to delete post: {response.StatusCode}\n{errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }


        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var post = button.DataContext as Post;
                if (post != null)
                {
                    this.NavigationService.Navigate(new EditPostPage(post));
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Dashboard());
        }
    }

    public class Post
    {
        public int PostId { get; set; }
        public int EmployerId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Likes { get; set; }
        public Employer Employer { get; set; }
    }

    public class Employer
    {
        public string CompanyName { get; set; }
    }

    public static class CurrentUser
    {
        public static int UserId { get; set; }
    }
}