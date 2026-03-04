using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    internal abstract class UserModel
    {
        // מחלקת המודל שבה יש את כל הנתונים על השחקן
        #region Enums
        protected enum Actions { Register, Login }
        #endregion

        #region Fields
        protected FbData fbd = new();
        protected Actions CurrentAction = Actions.Login;
        protected bool CanUseBiometrics { get; set; } = false;
        #endregion

        #region Events
        public EventHandler<bool>? OnAuthenticationComplete;
        public EventHandler? BiometricAvailabilityChanged;
        #endregion

        #region Properties
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = Preferences.Get(Keys.RememberMeKey, false);
        public bool IsBusy { get; protected set; } = false;
        public bool IsRegistered => !string.IsNullOrWhiteSpace(Preferences.Get(Keys.GmailKey, string.Empty));
        public abstract bool IsValidBiometric { get; }
        public abstract bool IsValidRegister { get; }
        public abstract bool IsValidLogin { get; }
        #endregion

        #region Public Methods
        public abstract void Register();
        public abstract void Login();
        public abstract void ResetPassword(string email);
        public abstract void SaveToPreferences();
        public abstract void CheckBiometricAvailability();
        public abstract void LoginWithBiometrics();
        #endregion

        #region Protected Methods
        protected abstract void ShowAlert(string message);
        protected abstract void OnCompleteRegister(Task task);
        protected abstract void OnCompleteLogin(Task task);
        protected abstract void OnCompleteResetPassword(Task task);
        protected abstract void OnCompleteBiometric(Task task);
        protected abstract void SaveToSecureStorage();
        #endregion
    }
}
