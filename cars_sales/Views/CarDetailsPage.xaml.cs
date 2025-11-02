using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using cars_sales.Models;
using cars_sales.Services;

namespace cars_sales.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CarDetailsPage : ContentPage
    {
        private Car _selectedCar;

        public CarDetailsPage(Car selectedCar)
        {
            InitializeComponent();
            _selectedCar = selectedCar;
            BindingContext = selectedCar;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Show role-based information
            if (UserService.IsAdmin)
            {
                DisplayAlert("Admin Access", "You have full access to view and manage this car.", "OK");
            }
            else
            {
                DisplayAlert("User Access", "You can view car details and contact the seller.", "OK");
            }
        }
    }
}
