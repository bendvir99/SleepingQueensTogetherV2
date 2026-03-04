using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public class LoginPageVM : ObservableObject
    {
        // הפעולה שמקשר את הלוגיקה של ההתחברות לדף ההתחברות
        #region Fields
        private readonly User user = new();
        #endregion

        #region ICommands
        public ICommand LoginCommand { get; }
        public ICommand NavigateToResetPasswordCommand { get; }
        public ICommand BiometricLoginCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        #endregion

        #region Properties
        public bool CanPressBiometric => CanBiometricLogin();
        public bool IsBusy => user.IsBusy;
        public bool IsPassword { get; private set; } = true;
        public bool IsRegistered => user.IsRegistered;
        public bool RememberMe
        {
            get => user.RememberMe;
            set 
            { 
                user.RememberMe = value;
                Preferences.Set(Keys.RememberMeKey, value);
            } 
        }
        public string Password
        {
            get => user.Password;
            set
            {
                if (user.Password != value)
                {
                    user.Password = value;
                    (LoginCommand as Command)?.ChangeCanExecute();
                }
            }
        }
        public string Email
        {
            get => user.Email;
            set
            {
                if (user.Email != value)
                {
                    user.Email = value;
                    (LoginCommand as Command)?.ChangeCanExecute();
                }
            }
        }
        #endregion

        #region Constructor
        public LoginPageVM()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            LoginCommand = new Command(Login, CanLogin);
            BiometricLoginCommand = new Command(BiometricLogin, CanBiometricLogin);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            NavigateToResetPasswordCommand = new Command(NavigateToResetPassword);
            user.OnAuthenticationComplete += OnAuthComplete;
            user.BiometricAvailabilityChanged += OnBiometricAvailabilityChange;
            user.CheckBiometricAvailability();
            if (Preferences.Get(Keys.RememberMeKey, false))
            {
                Password = Preferences.Get(Keys.PasswordKey, string.Empty);
                Email = Preferences.Get(Keys.GmailKey, string.Empty);
            }
        }
        #endregion

        #region Private Methods
        private void ToggleIsPassword()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }
        private void OnBiometricAvailabilityChange(object? sender, EventArgs e)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת הנתונים של האיוונט. הפעולה לא מחזירה שום ערך
            (BiometricLoginCommand as Command)?.ChangeCanExecute();
            OnPropertyChanged(nameof(CanPressBiometric));
        }
        private void NavigateToResetPassword()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (Application.Current != null)
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new PasswordResetPage();
                });
        }
        private void OnAuthComplete(object? sender, bool success)
        {
            // הפעולה מקבלת את מי ששלח את והאם ההתחברות הצליחה. הפעולה לא מחזירה שום ערך.
            if (success && Application.Current != null)
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new AppShell();
                });
            else
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    (LoginCommand as Command)?.ChangeCanExecute();
                });
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        private bool CanLogin()
        {
            // הפעולה לא מקבלת שום פרמטרים ומחזירה האם אפשר להתחבר
            return user.IsValidLogin;
        }
        private bool CanBiometricLogin()
        {
            // הפעולה לא מקבלת שום פרמטרים ומחזירה האם אפשר להתחבר בהתחברות הזאת.
            return user.IsValidBiometric;
        }
        private void Login()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (!IsBusy)
            {
                user.Login();
                OnPropertyChanged(nameof(IsBusy));
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }
        private void BiometricLogin()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (!IsBusy)
            {
                user.LoginWithBiometrics();
                OnPropertyChanged(nameof(IsBusy));
                (BiometricLoginCommand as Command)?.ChangeCanExecute();
            }
        }
        #endregion
    }
}
