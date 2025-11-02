using cars_sales.Models;
using System.Collections.Generic;
using System.Linq;

namespace cars_sales.Services
{
    public static class UserService
    {
        private static AppUser _currentUser;

        public static AppUser CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public static bool IsAdmin => _currentUser?.Role == UserRole.Admin;
        public static bool IsUser => _currentUser?.Role == UserRole.User;
        public static bool IsLoggedIn => _currentUser != null;

        // Seed users (in-memory demo)
        private static List<AppUser> GetSampleUsers()
        {
            return new List<AppUser>
            {
                new AppUser { Username = "admin", Password = "admin123", Role = UserRole.Admin, DisplayName = "Administrator" },
                new AppUser { Username = "john",  Password = "user123",  Role = UserRole.User,  DisplayName = "John Smith" },
                new AppUser { Username = "sarah", Password = "user123",  Role = UserRole.User,  DisplayName = "Sarah Johnson" },
                new AppUser { Username = "mike",  Password = "user123",  Role = UserRole.User,  DisplayName = "Mike Wilson" }
            };
        }

        // (Kept for compatibility with old role-button login)
        public static AppUser Login(string username)
        {
            var user = GetSampleUsers()
                .FirstOrDefault(u => u.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
            if (user != null) CurrentUser = user;
            return user;
        }

        // New: username + password
        public static AppUser Login(string username, string password)
        {
            var user = GetSampleUsers()
                .FirstOrDefault(u =>
                    u.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase) &&
                    u.Password == password);

            if (user != null) CurrentUser = user;
            return user;
        }

        public static void Logout() => CurrentUser = null;
    }
}
