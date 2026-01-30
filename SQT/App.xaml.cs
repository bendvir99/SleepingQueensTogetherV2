using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;

namespace SleepingQueensTogether
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            User user = new();
            Page page = user.IsRegistered ? new LoginPage() : new RegisterPage();
            MainPage = page;
        }
    }
}
