using AndroidX.Activity;
using CommunityToolkit.Maui.Alerts;
using Plugin.Fingerprint;
using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    class LoginPageVM : ObservableObject
    {
        private readonly User user = new();
        public ICommand LoginCommand { get; }
        public ICommand NavigateToResetPasswordCommand { get; }
        public ICommand BiometricLoginCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        public bool CanPressBiometric => CanBiometricLogin();
        public bool IsBusy => user.IsBusy;
        public bool IsPassword { get; set; } = true;
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
        private void ToggleIsPassword()
        {
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }
        public LoginPageVM()
        {
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

        private void OnBiometricAvailabilityChange(object? sender, EventArgs e)
        {
            (BiometricLoginCommand as Command)?.ChangeCanExecute();
            OnPropertyChanged(nameof(CanPressBiometric));
        }

        private void NavigateToResetPassword()
        {
            if (Application.Current != null)
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new PasswordResetPage();
                });
            }
        }

        private void OnAuthComplete(object? sender, bool success)
        {
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
            return user.IsValidLogin();
        }

        private bool CanBiometricLogin()
        {
            return user.IsValidBiometric();
        }

        private void Login()
        {
            if (!IsBusy)
            {
                user.Login();
                OnPropertyChanged(nameof(IsBusy));
                (LoginCommand as Command)?.ChangeCanExecute();
            }
        }
        private void BiometricLogin()
        {
            if (!IsBusy)
            {
                user.LoginWithBiometrics();
                OnPropertyChanged(nameof(IsBusy));
                (BiometricLoginCommand as Command)?.ChangeCanExecute();
            }
        }
    }
}
