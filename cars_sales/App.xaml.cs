using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using cars_sales.Views;
using cars_sales.Services;

namespace cars_sales
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
            MainPage = new NavigationPage(new cars_sales.Views.LoginPage());


            // Initialize database
            _ = InitializeDatabaseAsync();
        }
       


        private async Task InitializeDatabaseAsync()
        {
            try
            {
                await DatabaseService.InitializeDatabaseAsync();
                System.Diagnostics.Debug.WriteLine("Database initialized successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
