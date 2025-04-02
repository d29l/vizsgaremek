using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel
{
    public partial class EmployerRequestsPage : Page
    {
        public EmployerRequestsPage()
        {
            InitializeComponent();
            LoadRequests();
        }

        private async void LoadRequests()
        {
            try
            {
                var response = await ApiClient.httpClient.GetAsync("employerrequests/fetchRequests");

                if (response.IsSuccessStatusCode)
                {
                    var requests = await response.Content.ReadFromJsonAsync<List<EmployerRequest>>();
                    RequestsListView.ItemsSource = requests;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    RequestsListView.ItemsSource = null; // Clear the ListView if no requests exist
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error loading requests: {response.StatusCode}\n{errorContent}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                               "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int applicantId)
            {
                try
                {
                    // Step 1: Fetch the request
                    var requestResponse = await ApiClient.httpClient.GetAsync($"employerrequests/fetchRequest?applicantId={applicantId}");
                    if (!requestResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await requestResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Request not found: {requestResponse.StatusCode}\n{errorContent}",
                                       "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var request = await requestResponse.Content.ReadFromJsonAsync<EmployerRequest>();

                    // Step 2: Fetch the user
                    var userResponse = await ApiClient.httpClient.GetAsync($"users/fetchUser?userId={request.UserID}");
                    if (!userResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await userResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"User with ID {request.UserID} does not exist: {userResponse.StatusCode}\n{errorContent}",
                                       "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Step 3: Create employer
                    var createEmployerDto = new CreateEmployerDto
                    {
                        CompanyName = request.CompanyName,
                        CompanyAddress = request.CompanyAddress,
                        CompanyEmail = request.CompanyEmail,
                        CompanyPhoneNumber = request.CompanyPhoneNumber,
                        Industry = request.Industry,
                        CompanyWebsite = request.CompanyWebsite,
                        CompanyDescription = request.CompanyDescription,
                        EstablishedYear = request.EstablishedYear
                    };

                    var url = $"employers/postEmployer?UserId={request.UserID}";
                    var createResponse = await ApiClient.httpClient.PostAsJsonAsync(url, createEmployerDto);
                    if (!createResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await createResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to create employer: {createResponse.StatusCode}\n{errorContent}",
                                       "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Step 4: Delete the request
                    var deleteResponse = await ApiClient.httpClient.DeleteAsync($"employerrequests/deleteRequest?applicantId={applicantId}");
                    if (!deleteResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await deleteResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to delete request: {deleteResponse.StatusCode}\n{errorContent}",
                                       "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Step 5: Update user role
                    var roleUpdateUrl = $"users/updateUserRole/{request.UserID}?Role=Employer";
                    var roleUpdateResponse = await ApiClient.httpClient.PutAsync(roleUpdateUrl, null);
                    if (!roleUpdateResponse.IsSuccessStatusCode)
                    {
                        var errorContent = await roleUpdateResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to update user role: {roleUpdateResponse.StatusCode}\n{errorContent}",
                                       "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Success: Refresh list and notify user
                    LoadRequests();
                    MessageBox.Show("Employer created successfully and role updated to employer!",
                                   "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unexpected error occurred: {ex.Message}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid button or Tag property!",
                               "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int applicantId)
            {
                try
                {
                    var response = await ApiClient.httpClient.DeleteAsync($"employerrequests/deleteRequest?applicantId={applicantId}");

                    if (response.IsSuccessStatusCode)
                    {
                        LoadRequests();
                        MessageBox.Show("Request deleted successfully!",
                                       "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to delete request: {response.StatusCode}\n{errorContent}",
                                       "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Dashboard());
        }
    }

    public class EmployerRequest
    {
        public int ApplicantId { get; set; }
        public int UserID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public int? CompanyPhoneNumber { get; set; }
        public string Industry { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyDescription { get; set; }
        public int EstablishedYear { get; set; }
    }

    public class CreateEmployerDto
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }
        public int? CompanyPhoneNumber { get; set; }
        public string Industry { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyDescription { get; set; }
        public int EstablishedYear { get; set; }
    }
}
