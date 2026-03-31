using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public class PasswordResetPageVM : ObservableObject
    {
        // המחלקה שעוזרת לדף של השינוי סיסמה לפעול
        #region Fields
        private readonly User user = new();
        #endregion

        #region ICommands
        public ICommand ResetPasswordCommand { get; }
        public ICommand NavigateToLoginPageCommand { get; }
        #endregion

        #region Properties
        public string Email { get; set; } = string.Empty;
        public bool IsBusy => user.IsBusy;
        #endregion

        #region Constructor
        public PasswordResetPageVM()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            ResetPasswordCommand = new Command(ResetPassword);
            NavigateToLoginPageCommand = new Command(NavigateToLoginPage);
        }
        #endregion

        #region Private Methods
        private void NavigateToLoginPage()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (Application.Current != null)
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new LoginPage();
                });
        }
        private void ResetPassword()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            user.ResetPassword(Email);
        }
        #endregion
    }
}
