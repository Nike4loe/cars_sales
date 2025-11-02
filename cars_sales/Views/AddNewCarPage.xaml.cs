using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using cars_sales.Models;
using cars_sales.Services;

namespace cars_sales.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddNewCarPage : ContentPage
    {
        public event EventHandler<Car> CarAdded;
        private string _selectedImageName = null; // track chosen image

        public AddNewCarPage()
        {
            InitializeComponent();
        }

        private async void OnAddPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                // For now, just simulate picking a photo
                await DisplayAlert("Photo", "Photo picker would open here. Using placeholder image.", "OK");

                // Pretend user picked a photo → assign a placeholder
                _selectedImageName = "camera_placeholder.png";
                CarImage.Source = _selectedImageName;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error selecting photo: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // ✅ Validation
                if (string.IsNullOrWhiteSpace(MakeEntry.Text) ||
                    string.IsNullOrWhiteSpace(ModelEntry.Text) ||
                    string.IsNullOrWhiteSpace(YearEntry.Text) ||
                    string.IsNullOrWhiteSpace(PriceEntry.Text) ||
                    string.IsNullOrWhiteSpace(ColorEntry.Text))
                {
                    await DisplayAlert("Validation Error", "Please fill in all required fields (Make, Model, Year, Price, Color).", "OK");
                    return;
                }

                if (!int.TryParse(YearEntry.Text, out int year) || year < 1900 || year > DateTime.Now.Year)
                {
                    await DisplayAlert("Validation Error", "Please enter a valid year.", "OK");
                    return;
                }

                if (!decimal.TryParse(PriceEntry.Text, out decimal price) || price <= 0)
                {
                    await DisplayAlert("Validation Error", "Please enter a valid price.", "OK");
                    return;
                }

                // ✅ Create new car
                var newCar = new Car
                {
                    Make = MakeEntry.Text.Trim(),
                    Model = ModelEntry.Text.Trim(),
                    Year = year,
                    Price = price,
                    Color = ColorEntry.Text?.Trim() ?? "N/A",
                    Mileage = MileageEntry.Text?.Trim() ?? "N/A",
                    FuelType = FuelTypePicker.SelectedItem?.ToString() ?? "Gasoline",
                    Transmission = TransmissionPicker.SelectedItem?.ToString() ?? "Automatic",
                    Description = DescriptionEditor.Text?.Trim() ?? "No description provided.",
                    ImageName = string.IsNullOrWhiteSpace(_selectedImageName) ? "camera_placeholder.png" : _selectedImageName,
                    CreatedBy = UserService.CurrentUser?.Username ?? "Unknown"
                };

                CarAdded?.Invoke(this, newCar);

                await DisplayAlert("Success", $"{newCar.Make} {newCar.Model} added successfully!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error saving car: {ex.Message}", "OK");
            }
        }
    }
}
