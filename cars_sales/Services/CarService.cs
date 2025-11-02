using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cars_sales.Models;

namespace cars_sales.Services
{
    public static class CarService
    {
        // Database-based methods
        public static async Task<List<Car>> GetAllCarsAsync()
        {
            return await DatabaseService.GetAllCarsAsync();
        }

        public static async Task<Car> GetCarByIdAsync(int id)
        {
            return await DatabaseService.GetCarByIdAsync(id);
        }

        public static async Task<int> AddCarAsync(Car car)
        {
            car.CreatedBy = UserService.CurrentUser?.Username ?? "Unknown";
            return await DatabaseService.InsertCarAsync(car);
        }

        public static async Task<int> UpdateCarAsync(Car car)
        {
            return await DatabaseService.UpdateCarAsync(car);
        }

        public static async Task<int> DeleteCarAsync(int id)
        {
            return await DatabaseService.DeleteCarAsync(id);
        }

        public static async Task<List<Car>> SearchCarsAsync(string searchText)
        {
            return await DatabaseService.SearchCarsAsync(searchText);
        }

        public static async Task<List<Car>> FilterCarsAsync(string filter, string searchText = null)
        {
            return await DatabaseService.FilterCarsAsync(filter, searchText);
        }

        // Legacy synchronous methods for backward compatibility
        public static List<Car> GetSampleCars()
        {
            // This method is kept for backward compatibility but should not be used
            // The app should use async methods instead
            return new List<Car>();
        }

        public static List<Car> FilterCars(List<Car> cars, string searchText, string filter)
        {
            // Legacy method - kept for backward compatibility
            searchText = searchText?.ToLower() ?? "";

            return cars.Where(car =>
            {
                bool matchesSearch = string.IsNullOrEmpty(searchText) ||
                    car.Make.ToLower().Contains(searchText) ||
                    car.Model.ToLower().Contains(searchText) ||
                    car.Year.ToString().Contains(searchText);

                bool matchesCategory = filter switch
                {
                    "Under $30K" => car.Price < 30000,
                    "2022+" => car.Year >= 2022,
                    "Luxury" => car.Make == "BMW" || car.Make == "Tesla" || car.Price > 40000,
                    "Electric" => car.FuelType == "Electric",
                    "Gasoline" => car.FuelType == "Gasoline",
                    _ => true
                };

                return matchesSearch && matchesCategory;
            }).ToList();
        }

        // Statistics methods
        public static async Task<int> GetTotalCarsCountAsync()
        {
            return await DatabaseService.GetTotalCarsCountAsync();
        }

        public static async Task<decimal> GetAveragePriceAsync()
        {
            return await DatabaseService.GetAveragePriceAsync();
        }

        public static async Task<List<string>> GetUniqueMakesAsync()
        {
            return await DatabaseService.GetUniqueMakesAsync();
        }
    }
}
