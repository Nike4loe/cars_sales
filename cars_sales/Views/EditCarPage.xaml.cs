using cars_sales.Models;
using cars_sales.Services;
using System;
using Xamarin.Forms;

namespace cars_sales.Views
{
    public partial class EditCarPage : ContentPage
    {
        public event EventHandler<Car> CarUpdated;
        private Car _originalCar;
        private string _selectedImageName;

        public EditCarPage(Car carToEdit)
        {
            InitializeComponent();
            _originalCar = carToEdit;
            BindingContext = carToEdit;
            _selectedImageName = carToEdit.ImageName;
        }

        private async void OnAddPhotoClicked(object sender, EventArgs e)
        {
            // For now, just cycle through available images
            var images = new[] { "car_camry.png", "car_civic.png", "car_f150.png", "car_x5.png", "car_model3.png" };
            var currentIndex = Array.IndexOf(images, _selectedImageName);
            var nextIndex = (currentIndex + 1) % images.Length;
            _selectedImageName = images[nextIndex];
            
            CarImage.Source = _selectedImageName;
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(MakeEntry.Text) || string.IsNullOrWhiteSpace(ModelEntry.Text))
                {
                    await DisplayAlert("Error", "Make and Model are required fields.", "OK");
                    return;
                }

                if (!int.TryParse(YearEntry.Text, out int year) || year < 1900 || year > DateTime.Now.Year + 1)
                {
                    await DisplayAlert("Error", "Please enter a valid year.", "OK");
                    return;
                }

                if (!decimal.TryParse(PriceEntry.Text, out decimal price) || price <= 0)
                {
                    await DisplayAlert("Error", "Please enter a valid price.", "OK");
                    return;
                }

                // Create updated car
                var updatedCar = new Car
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
                    ImageName = string.IsNullOrWhiteSpace(_selectedImageName) ? "camera_placeholder.png" : _selectedImageName
                };

                CarUpdated?.Invoke(this, updatedCar);

                await DisplayAlert("Success", $"{updatedCar.Make} {updatedCar.Model} updated successfully!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error updating car: {ex.Message}", "OK");
            }
        }
    }
}
