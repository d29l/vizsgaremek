using System;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel
{
    public partial class EditPostPage : Page
    {
        private Post _post;

        public EditPostPage(Post post)
        {
            InitializeComponent();
            _post = post;
            LoadPostData();
        }

        private void LoadPostData()
        {
            TitleTextBox.Text = _post.Title;
            CategoryTextBox.Text = _post.Category;
            LocationTextBox.Text = _post.Location;
            ContentTextBox.Text = _post.Content;
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            var updatePostDto = new UpdatePostDto
            {
                Title = TitleTextBox.Text,
                Category = CategoryTextBox.Text,
                Location = LocationTextBox.Text,
                Content = ContentTextBox.Text
            };

            var url = $"posts/editPost?PostId={_post.PostId}&EmployerId={_post.EmployerId}&userId={CurrentUser.UserId}";
            try
            {
                var response = await ApiClient.httpClient.PutAsJsonAsync(url, updatePostDto);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Post updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.NavigationService.GoBack();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to update post: {response.StatusCode}\n{errorContent}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
        
    }

    public class UpdatePostDto
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Content { get; set; }
    }
}