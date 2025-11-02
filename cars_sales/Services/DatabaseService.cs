using SQLite;
using cars_sales.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace cars_sales.Services
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;
        private static readonly object _lock = new object();

        public static SQLiteAsyncConnection Database
        {
            get
            {
                if (_database == null)
                {
                    lock (_lock)
                    {
                        if (_database == null)
                        {
                            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "cars_sales.db3");
                            _database = new SQLiteAsyncConnection(dbPath);
                        }
                    }
                }
                return _database;
            }
        }

        public static async Task InitializeDatabaseAsync()
        {
            try
            {
                await Database.CreateTableAsync<Car>();
                
                // Check if we need to seed initial data
                var carCount = await Database.Table<Car>().CountAsync();
                if (carCount == 0)
                {
                    await SeedInitialDataAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error or handle appropriately
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        private static async Task SeedInitialDataAsync()
        {
            var sampleCars = new List<Car>
            {
                new Car 
                { 
                    Make = "Toyota", 
                    Model = "Camry", 
                    Year = 2022, 
                    Price = 25000, 
                    Color = "Silver",
                    ImageName = "car_camry.png",
                    Description = "Reliable and fuel-efficient sedan perfect for daily commuting.",
                    Mileage = "15,000 miles", 
                    FuelType = "Gasoline", 
                    Transmission = "Automatic",
                    CreatedBy = "System"
                },
                new Car 
                { 
                    Make = "Honda", 
                    Model = "Civic", 
                    Year = 2021, 
                    Price = 22000, 
                    Color = "Blue",
                    ImageName = "car_civic.png",
                    Description = "Sporty and efficient compact car with excellent resale value.",
                    Mileage = "18,500 miles", 
                    FuelType = "Gasoline", 
                    Transmission = "CVT",
                    CreatedBy = "System"
                },
                new Car 
                { 
                    Make = "Ford", 
                    Model = "F-150", 
                    Year = 2023, 
                    Price = 35000, 
                    Color = "Red",
                    ImageName = "car_f150.png",
                    Description = "Powerful pickup truck perfect for work and recreation.",
                    Mileage = "8,000 miles", 
                    FuelType = "Gasoline", 
                    Transmission = "Automatic",
                    CreatedBy = "System"
                },
                new Car 
                { 
                    Make = "BMW", 
                    Model = "X5", 
                    Year = 2022, 
                    Price = 55000, 
                    Color = "Black",
                    ImageName = "car_x5.png",
                    Description = "Luxury SUV with premium features and excellent performance.",
                    Mileage = "12,000 miles", 
                    FuelType = "Gasoline", 
                    Transmission = "Automatic",
                    CreatedBy = "System"
                },
                new Car 
                { 
                    Make = "Tesla", 
                    Model = "Model 3", 
                    Year = 2023, 
                    Price = 45000, 
                    Color = "White",
                    ImageName = "car_model3.png",
                    Description = "Electric vehicle with cutting-edge technology and zero emissions.",
                    Mileage = "5,000 miles", 
                    FuelType = "Electric", 
                    Transmission = "Automatic",
                    CreatedBy = "System"
                }
            };

            foreach (var car in sampleCars)
            {
                await Database.InsertAsync(car);
            }
        }

        // Car CRUD operations
        public static async Task<List<Car>> GetAllCarsAsync()
        {
            return await Database.Table<Car>()
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public static async Task<Car> GetCarByIdAsync(int id)
        {
            return await Database.Table<Car>()
                .Where(c => c.Id == id && c.IsActive)
                .FirstOrDefaultAsync();
        }

        public static async Task<int> InsertCarAsync(Car car)
        {
            car.CreatedDate = DateTime.Now;
            car.IsActive = true;
            return await Database.InsertAsync(car);
        }

        public static async Task<int> UpdateCarAsync(Car car)
        {
            car.UpdatedDate = DateTime.Now;
            return await Database.UpdateAsync(car);
        }

        public static async Task<int> DeleteCarAsync(int id)
        {
            var car = await GetCarByIdAsync(id);
            if (car != null)
            {
                car.IsActive = false;
                car.UpdatedDate = DateTime.Now;
                return await UpdateCarAsync(car);
            }
            return 0;
        }

        public static async Task<List<Car>> SearchCarsAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return await GetAllCarsAsync();

            return await Database.Table<Car>()
                .Where(c => c.IsActive && 
                    (c.Make.ToLower().Contains(searchText.ToLower()) ||
                     c.Model.ToLower().Contains(searchText.ToLower()) ||
                     c.Year.ToString().Contains(searchText)))
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public static async Task<List<Car>> FilterCarsAsync(string filter, string searchText = null)
        {
            var query = Database.Table<Car>().Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(c => 
                    c.Make.ToLower().Contains(searchText.ToLower()) ||
                    c.Model.ToLower().Contains(searchText.ToLower()) ||
                    c.Year.ToString().Contains(searchText));
            }

            switch (filter)
            {
                case "Under $30K":
                    query = query.Where(c => c.Price < 30000);
                    break;
                case "2022+":
                    query = query.Where(c => c.Year >= 2022);
                    break;
                case "Luxury":
                    query = query.Where(c => c.Make == "BMW" || c.Make == "Tesla" || c.Price > 40000);
                    break;
                case "Electric":
                    query = query.Where(c => c.FuelType == "Electric");
                    break;
                case "Gasoline":
                    query = query.Where(c => c.FuelType == "Gasoline");
                    break;
            }

            return await query.OrderByDescending(c => c.CreatedDate).ToListAsync();
        }

        // Statistics
        public static async Task<int> GetTotalCarsCountAsync()
        {
            return await Database.Table<Car>().Where(c => c.IsActive).CountAsync();
        }

        public static async Task<decimal> GetAveragePriceAsync()
        {
            var cars = await Database.Table<Car>().Where(c => c.IsActive).ToListAsync();
            return cars.Any() ? cars.Average(c => c.Price) : 0;
        }

        public static async Task<List<string>> GetUniqueMakesAsync()
        {
            var cars = await Database.Table<Car>()
                .Where(c => c.IsActive)
                .ToListAsync();
            return cars.Select(c => c.Make).Distinct().OrderBy(m => m).ToList();
        }
    }
}
