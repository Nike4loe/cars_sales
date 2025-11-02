namespace cars_sales.Models
{
    public enum UserRole
    {
        Admin,
        User
    }

    public class AppUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string DisplayName { get; set; }
    }
}
