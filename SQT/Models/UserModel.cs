using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    internal abstract class UserModel
    {
        protected FbData fbd = new();
        protected enum Actions { Register, Login }
        protected Actions CurrentAction = Actions.Login;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public EventHandler<bool>? OnAuthenticationComplete;
        public EventHandler? BiometricAvailabilityChanged;
        public bool RememberMe { get; set; } = Preferences.Get(Keys.RememberMeKey, false);
        public bool IsBusy { get; protected set; } = false;
        public bool IsRegistered => !string.IsNullOrWhiteSpace(Preferences.Get(Keys.GmailKey, string.Empty));
        public abstract bool IsValidRegister();
        public abstract bool IsValidLogin();
        public abstract bool IsValidBiometric();
        public abstract void Register();
        public abstract void Login();
        public abstract void ResetPassword(string email);
        public abstract void SaveToPreferences();
        public abstract void CheckBiometricAvailability();
        public abstract void LoginWithBiometrics();
        protected abstract void ShowAlert(string message);
        protected abstract void OnCompleteRegister(Task task);
        protected abstract void OnCompleteLogin(Task task);
        protected abstract void OnCompleteResetPassword(Task task);
        protected abstract void OnCompleteBiometric(Task task);
        protected abstract void SaveToSecureStorage();
    }
}
