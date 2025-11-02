using cars_sales.Models;
using cars_sales.Services;
using cars_sales.ViewModels;
using cars_sales.Views;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml; // ✅ Generic FindByName<T>

namespace cars_sales   // 🔁 Ensure this matches MainPage.xaml's x:Class (cars_sales.MainPage)
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel ViewModel;

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();
            BindingContext = ViewModel;

            EnsureAdminUI();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Make sure we're using the correct view model instance
            BindingContext = ViewModel;

            // 🔹 Refresh all role-based properties (IsAdmin, IsUser, etc.)
            ViewModel.RefreshRoleBindings();

            // 🔹 Then adjust UI visibility (toolbar, badges, buttons)
            EnsureAdminUI();
        }


        /// <summary>
        /// Show/hide/enable admin-only UI.
        /// </summary>
        private void EnsureAdminUI()
        {
            // Admin badge
            var adminBadge = this.FindByName<VisualElement>("AdminBadge");
            if (adminBadge != null)
                adminBadge.IsVisible = UserService.IsAdmin;

            // Optional Add Car button in content
            var addBtn = this.FindByName<Button>("AddCarButton");
            if (addBtn != null)
                addBtn.IsVisible = UserService.IsAdmin;

            // Toolbar Add Car
            var addCarTI = this.FindByName<ToolbarItem>("AddCarToolbarItem");
            if (addCarTI != null)
            {
                // Keep the button but disable if not admin
                addCarTI.IsEnabled = UserService.IsAdmin;

                // If you want to completely hide it instead, use add/remove:
                /*
                bool exists = ToolbarItems.Contains(addCarTI);
                if (UserService.IsAdmin && !exists)
                    ToolbarItems.Add(addCarTI);
                else if (!UserService.IsAdmin && exists)
                    ToolbarItems.Remove(addCarTI);
                */
            }
        }

        private async void OnAddCarClicked(object sender, EventArgs e)
        {
            if (!UserService.IsAdmin)
            {
                await DisplayAlert("Access Denied", "Only administrators can add cars.", "OK");
                return;
            }

            var addCarPage = new AddNewCarPage();
            addCarPage.CarAdded += async (s, newCar) =>
            {
                await ViewModel.AddCarAsync(newCar);
            };
            await Navigation.PushAsync(addCarPage);
        }

        private async void OnCarTapped(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Car selectedCar)
                await Navigation.PushAsync(new CarDetailsPage(selectedCar));

            if (sender is CollectionView cv)
                cv.SelectedItem = null;
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            await ViewModel.SearchAsync(e.NewTextValue);
        }

        private async void OnFilterClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var searchBar = this.FindByName<SearchBar>("CarSearchBar");
            string query = searchBar?.Text ?? string.Empty;

            await ViewModel.FilterAsync(button.Text, query);
        }

        private async void OnEditCarClicked(object sender, EventArgs e)
        {
            if (!UserService.IsAdmin)
            {
                await DisplayAlert("Access Denied", "Only administrators can edit cars.", "OK");
                return;
            }

            if ((sender as Button)?.CommandParameter is Car carToEdit)
            {
                var editCarPage = new EditCarPage(carToEdit);
                editCarPage.CarUpdated += async (s, updatedCar) =>
                {
                    await ViewModel.UpdateCarAsync(carToEdit, updatedCar);
                };
                await Navigation.PushAsync(editCarPage);
            }
        }

        private async void OnDeleteCarClicked(object sender, EventArgs e)
        {
            if (!UserService.IsAdmin)
            {
                await DisplayAlert("Access Denied", "Only administrators can delete cars.", "OK");
                return;
            }

            if ((sender as Button)?.CommandParameter is Car carToDelete)
            {
                var result = await DisplayAlert(
                    "Delete Car",
                    $"Are you sure you want to delete {carToDelete.Make} {carToDelete.Model}?",
                    "Delete", "Cancel");

                if (result)
                {
                    await ViewModel.DeleteCarAsync(carToDelete);
                    await DisplayAlert("Success", "Car deleted successfully!", "OK");
                }
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("Logout", "Do you want to sign out?", "Yes", "No");
            if (!confirm) return;

            UserService.Logout();

            // Reset the root to avoid back navigation into authenticated pages
            Application.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
