using cars_sales.Models;
using cars_sales.Services;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace cars_sales.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<Car> _cars;
        public List<Car> Cars
        {
            get => _cars;
            set { _cars = value; OnPropertyChanged(); }
        }

        private List<Car> _filteredCars;
        private string _currentFilter = "All";

        public List<Car> FilteredCars
        {
            get => _filteredCars;
            set { _filteredCars = value; OnPropertyChanged(); }
        }

        // ---------- Role-based properties (bind to these in XAML) ----------
        public bool IsAdmin => UserService.IsAdmin;
        public bool IsUser => UserService.IsUser;
        public string CurrentUserDisplayName => UserService.CurrentUser?.DisplayName ?? "Guest";

        public string RoleDisplayText => IsAdmin ? "👨‍💼 Administrator Mode" : "👤 User Mode";
        public Color RoleColor => IsAdmin ? Color.FromHex("#2196F3") : Color.FromHex("#9C27B0");

        // Call this whenever the logged-in user/role might have changed
        public void RefreshRoleBindings()
        {
            OnPropertyChanged(nameof(IsAdmin));
            OnPropertyChanged(nameof(IsUser));
            OnPropertyChanged(nameof(CurrentUserDisplayName));
            OnPropertyChanged(nameof(RoleDisplayText));
            OnPropertyChanged(nameof(RoleColor));
        }
        // ------------------------------------------------------------------

        // Loading state
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public MainPageViewModel()
        {
            Cars = new List<Car>();
            FilteredCars = new List<Car>();
            _ = LoadCarsAsync();

            // ensure initial role state is reflected in UI
            RefreshRoleBindings();
        }

        public async Task LoadCarsAsync()
        {
            try
            {
                IsLoading = true;
                Cars = await CarService.GetAllCarsAsync();
                FilteredCars = new List<Car>(Cars);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cars: {ex.Message}");
                Cars = new List<Car>();
                FilteredCars = new List<Car>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SearchAsync(string searchText)
        {
            try
            {
                IsLoading = true;
                if (string.IsNullOrWhiteSpace(searchText))
                    FilteredCars = new List<Car>(Cars);
                else
                    FilteredCars = await CarService.SearchCarsAsync(searchText);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error searching cars: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task FilterAsync(string filter, string searchText)
        {
            try
            {
                IsLoading = true;
                _currentFilter = filter;
                FilteredCars = await CarService.FilterCarsAsync(filter, searchText);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error filtering cars: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Admin methods
        public async Task AddCarAsync(Car newCar)
        {
            if (!IsAdmin) return;

            try
            {
                IsLoading = true;
                var result = await CarService.AddCarAsync(newCar);
                if (result > 0)
                    await LoadCarsAsync(); // Reload all cars
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding car: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task DeleteCarAsync(Car carToDelete)
        {
            if (!IsAdmin || carToDelete == null) return;

            try
            {
                IsLoading = true;
                var result = await CarService.DeleteCarAsync(carToDelete.Id);
                if (result > 0)
                    await LoadCarsAsync();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting car: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UpdateCarAsync(Car originalCar, Car updatedCar)
        {
            if (!IsAdmin) return;

            try
            {
                IsLoading = true;
                var result = await CarService.UpdateCarAsync(updatedCar);
                if (result > 0)
                    await LoadCarsAsync();
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating car: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Legacy synchronous wrappers
        public void Search(string searchText) => _ = SearchAsync(searchText);
        public void Filter(string filter, string searchText) => _ = FilterAsync(filter, searchText);
        public void AddCar(Car newCar) => _ = AddCarAsync(newCar);
        public void DeleteCar(Car carToDelete) => _ = DeleteCarAsync(carToDelete);
        public void UpdateCar(Car originalCar, Car updatedCar) => _ = UpdateCarAsync(originalCar, updatedCar);

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
