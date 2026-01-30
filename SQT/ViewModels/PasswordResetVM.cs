using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public class PasswordResetVM
    {
        private readonly User user = new();
        public string Email { get; set; } = string.Empty;
        public bool IsBusy => user.IsBusy;
        public ICommand ResetPasswordCommand { get; }
        public ICommand NavigateToLoginPageCommand { get; }
        public PasswordResetVM()
        {
            ResetPasswordCommand = new Command(ResetPassword);
            NavigateToLoginPageCommand = new Command(NavigateToLoginPage);
        }

        private void NavigateToLoginPage()
        {
            if (Application.Current != null)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new LoginPage();
                });
            }
        }

        private void ResetPassword()
        {
            user.ResetPassword(Email);
        }
    }
}
