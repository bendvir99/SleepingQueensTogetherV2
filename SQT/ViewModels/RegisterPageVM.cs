using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public class RegisterPageVM : ObservableObject
    {
        // המחלקה שעוזרת לדף רישום לפעול בעזרת קישור ללוגיקה
        #region Fields
        private readonly User user = new();
        #endregion

        #region ICommands
        public ICommand RegisterCommand { get; }
        public ICommand ToggleIsPasswordCommand { get; }
        #endregion

        #region Properties
        public bool IsBusy => user.IsBusy;
        public bool IsPassword { get; set; } = true;
        public bool IsRegistered => user.IsRegistered;
        public string Name
        { 
            get => user.Name;
            set
            {
                if (user.Name != value)
                {
                    user.Name = value;
                    (RegisterCommand as Command)?.ChangeCanExecute();
                }
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
                    (RegisterCommand as Command)?.ChangeCanExecute();
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
                    (RegisterCommand as Command)?.ChangeCanExecute();
                }
            }
        }
        #endregion

        #region Constructor
        public RegisterPageVM()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            RegisterCommand = new Command(Register, CanRegister);
            ToggleIsPasswordCommand = new Command(ToggleIsPassword);
            user.OnAuthenticationComplete += OnAuthComplete;
        }
        #endregion

        #region Private Methods
        private void ToggleIsPassword()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            IsPassword = !IsPassword;
            OnPropertyChanged(nameof(IsPassword));
        }
        private void OnAuthComplete(object? sender, bool success)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט והאם ההתחברות הצליחה. הפעולה לא מחזירה שום ערך
            OnPropertyChanged(nameof(IsBusy));
            if (success && Application.Current != null)
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Application.Current.MainPage = new AppShell();
                });
            else
            {
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(Name));
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    (RegisterCommand as Command)?.ChangeCanExecute();
                });
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        private bool CanRegister()
        {
            // הפעולה לא מקבלת שום פרמטרים ומחזירה האם אפשר להירשם
            return user.IsValidRegister;
        }

        private void Register()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (!IsBusy)
            {
                user.Register();
                OnPropertyChanged(nameof(IsBusy));
                (RegisterCommand as Command)?.ChangeCanExecute();
            }
        }
        #endregion
    }
}
