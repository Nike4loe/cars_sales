using System;
using SQLite;

namespace cars_sales.Models
{
    [Table("Cars")]
    public class Car
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [NotNull]
        public string Make { get; set; }
        
        [NotNull]
        public string Model { get; set; }
        
        [NotNull]
        public int Year { get; set; }
        
        [NotNull]
        public decimal Price { get; set; }
        
        [NotNull]
        public string Color { get; set; }
        
        public string ImageName { get; set; }
        public string Description { get; set; }
        public string Mileage { get; set; }
        public string FuelType { get; set; }
        public string Transmission { get; set; }
        
        // Additional database fields
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; } = true;

        // ✅ Always return a valid image
        public string DisplayImage
        {
            get
            {
                return string.IsNullOrWhiteSpace(ImageName) ? "camera_placeholder.png" : ImageName;
            }
        }
        
        // Helper property for display
        [Ignore]
        public string FullName => $"{Make} {Model} ({Year})";
        
        [Ignore]
        public string FormattedPrice => $"${Price:N0}";
    }
}
