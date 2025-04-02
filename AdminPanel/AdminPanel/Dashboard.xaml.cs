using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page, System.ComponentModel.INotifyPropertyChanged
    {
        private int _userCount;
        private int _requestCount;
        private int _postCount;

        public int userCount
        {
            get => _userCount;
            set
            {
                _userCount = value;
                OnPropertyChanged(nameof(userCount));
            }
        }

        public int requestCount
        {
            get => _requestCount;
            set
            {
                _requestCount = value;
                OnPropertyChanged(nameof(requestCount));
            }
        }

        public int postCount
        {
            get => _postCount;
            set
            {
                _postCount = value;
                OnPropertyChanged(nameof(postCount));
            }
        }
        

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        public Dashboard()
        {
            InitializeComponent();
            DataContext = this;
            LoadCounts();

            
            WelcomeText.Text = "Welcome " + CurrentUser.UserName;
        }

        
        private async void LoadCounts()
        {
            try
            {
                // Fetch user count
                var userResponse = await ApiClient.httpClient.GetAsync("users/fetchUsers");
                if (userResponse.IsSuccessStatusCode)
                {
                    var users = await userResponse.Content.ReadFromJsonAsync<List<User>>();
                    userCount = users?.Count ?? 0;
                }
                else
                {
                    userCount = 0;
                    MessageBox.Show($"Error fetching users: {userResponse.StatusCode}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Fetch request count with NotFound handling (set count to 0 if no requests exist)
                var requestResponse = await ApiClient.httpClient.GetAsync("employerrequests/fetchRequests");
                if (requestResponse.IsSuccessStatusCode)
                {
                    var requests = await requestResponse.Content.ReadFromJsonAsync<List<EmployerRequest>>();
                    requestCount = requests?.Count ?? 0;
                }
                else if (requestResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    requestCount = 0;
                }
                else
                {
                    requestCount = 0;
                    MessageBox.Show($"Error fetching requests: {requestResponse.StatusCode}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                // Fetch post count
                var postResponse = await ApiClient.httpClient.GetAsync("posts/fetchPosts");
                if (postResponse.IsSuccessStatusCode)
                {
                    var posts = await postResponse.Content.ReadFromJsonAsync<List<Post>>();
                    postCount = posts?.Count ?? 0;
                }
                else
                {
                    postCount = 0;
                    MessageBox.Show($"Error fetching posts: {postResponse.StatusCode}",
                                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                userCount = requestCount = postCount = 0;
                MessageBox.Show($"Error loading counts: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ManageRequests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployerRequestsPage());
        }

        private void ViewUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UsersPage());
        }

        private void ViewPosts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PostsPage());
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            
            App.CurrentUserAccessToken = null;
            App.CurrentUserRefreshToken = null;
            CurrentUser.UserId = 0;

            
            this.NavigationService.Navigate(new LoginPage()); 
        }
    }
}
