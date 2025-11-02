using cars_sales.Models;
using cars_sales.Services;
using System;
using Xamarin.Forms;

namespace cars_sales.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            try
            {
                Busy.IsVisible = Busy.IsRunning = true;

                var username = UsernameEntry?.Text?.Trim();
                var password = PasswordEntry?.Text?.Trim();

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    await DisplayAlert("Missing information", "Please enter both username and password.", "OK");
                    return;
                }

                var user = UserService.Login(username, password);
                if (user == null)
                {
                    await DisplayAlert("Login failed", "Invalid username or password.", "OK");
                    return;
                }

                await DisplayAlert("Welcome", $"Hello {user.DisplayName}", "OK");

                // Route by role (you can make a dedicated AdminDashboardPage if you want)
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            finally
            {
                Busy.IsRunning = Busy.IsVisible = false;
            }
        }
    }
}
