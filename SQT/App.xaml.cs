using Microsoft.Maui.ApplicationModel;
using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;

namespace SleepingQueensTogether
{
    public partial class App : Application
    {
        public App()
        {
            if (Current != null)
                Current.UserAppTheme = AppTheme.Light;
            InitializeComponent();
            User user = new();
            Page page = user.IsRegistered ? new LoginPage() : new RegisterPage();
            MainPage = page;
        }
    }
}
