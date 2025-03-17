using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
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
                else
                {
                    MessageBox.Show($"Error loading requests: {response.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int applicantId)
            {
                try
                {
                    
                    var requestResponse = await ApiClient.httpClient.GetAsync($"employerrequests/fetchRequest/{applicantId}");
                    if (!requestResponse.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Request not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var request = await requestResponse.Content.ReadFromJsonAsync<EmployerRequest>();

                    
                    var userResponse = await ApiClient.httpClient.GetAsync($"users/fetchUser/{request.UserID}");
                    if (!userResponse.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"User with ID {request.UserID} does not exist!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    
                    var createEmployerDto = new CreateEmployerDto
                    {
                        CompanyName = request.CompanyName,
                        CompanyAddress = request.CompanyAddress,
                        Industry = request.Industry,
                        CompanyWebsite = request.CompanyWebsite,
                        CompanyDescription = request.CompanyDescription,
                        EstablishedYear = request.EstablishedYear
                    };

                    
                    var url = $"employers/postEmployer?UserId={request.UserID}";
                    var createResponse = await ApiClient.httpClient.PostAsJsonAsync(url, createEmployerDto);

                    if (createResponse.IsSuccessStatusCode)
                    {
                        
                        await ApiClient.httpClient.DeleteAsync($"employerrequests/deleteRequest/{applicantId}");
                        LoadRequests(); 
                        MessageBox.Show("Employer created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        
                        var updateRoleUrl = $"api/user/updateUserRole?UserId={request.UserID}";
                        var roleUpdateResponse = await ApiClient.httpClient.PutAsJsonAsync(updateRoleUrl, new { Role = "employer" });

                        if (!roleUpdateResponse.IsSuccessStatusCode)
                        {
                            var errorContent = await roleUpdateResponse.Content.ReadAsStringAsync();
                            MessageBox.Show($"Failed to update user role: {roleUpdateResponse.StatusCode}\n{errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        var errorContent = await createResponse.Content.ReadAsStringAsync();
                        MessageBox.Show($"Failed to create employer: {createResponse.StatusCode}\n{errorContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid button or Tag property!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int applicantId)
            {
                try
                {
                    var response = await ApiClient.httpClient.DeleteAsync($"employerrequests/deleteRequest/{applicantId}");

                    if (response.IsSuccessStatusCode)
                    {
                        LoadRequests();
                        MessageBox.Show("Request deleted!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class EmployerRequest
    {
        public int ApplicantId { get; set; }
        public int UserID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Industry { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyDescription { get; set; }
        public int EstablishedYear { get; set; }
    }

    public class CreateEmployerDto
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string Industry { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyDescription { get; set; }
        public int EstablishedYear { get; set; }
    }
}