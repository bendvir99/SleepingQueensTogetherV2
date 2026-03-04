using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    internal class User : UserModel
    {
        // במחלקה הזאת מתנהל כל הפעולות של השחקן וכל מה שאפשר לעשות על השחקן
        #region Properties
        public override bool IsValidBiometric => CanUseBiometrics && !IsBusy;
        public override bool IsValidRegister => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email) && !IsBusy;
        public override bool IsValidLogin => !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email) && !IsBusy;
        #endregion

        #region Public Methods
        public override void Register()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            IsBusy = true;
            CurrentAction = Actions.Register;
            fbd.CreateUserWithEmailAndPasswordAsync(Email, Password, Name,OnCompleteRegister);
        }
        public override void Login()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            IsBusy = true;
            fbd.SignInWithEmailAndPasswordAsync(Email, Password, OnCompleteLogin);
        }
        public override void CheckBiometricAvailability()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            _ = Task.Run(async () =>
            {
                string? savedEmail = await SecureStorage.GetAsync(Keys.LastEmailKey);
                string? savedPassword = await SecureStorage.GetAsync(Keys.LastPasswordKey);
                bool biometricAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);
                CanUseBiometrics = biometricAvailable &&
                                     !string.IsNullOrEmpty(savedEmail) &&
                                     !string.IsNullOrEmpty(savedPassword);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    BiometricAvailabilityChanged?.Invoke(this, EventArgs.Empty);
                });
            });
        }
        public override async void LoginWithBiometrics()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            string? email = await SecureStorage.GetAsync(Keys.LastEmailKey);
            string? password = await SecureStorage.GetAsync(Keys.LastPasswordKey);
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                AuthenticationRequestConfiguration authRequest = new(
                    Strings.unlock,
                    Strings.confirmIdentity
                );
                FingerprintAuthenticationResult result = await CrossFingerprint.Current.AuthenticateAsync(authRequest);
                if (result.Authenticated)
                {
                    IsBusy = true;
                    fbd.SignInWithEmailAndPasswordAsync(email, password, OnCompleteBiometric);
                }
            }
        }
        public override void ResetPassword(string email)
        {
            // הפעולה מקבלת את האימייל ולא מחזירה שום ערך
            IsBusy = true;
            fbd.ResetPasswordWithEmail(email, OnCompleteResetPassword);
        }
        public override void SaveToPreferences()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            Preferences.Set(Keys.UsernameKey, Name);
            Preferences.Set(Keys.GmailKey, Email);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.RememberMeKey, RememberMe);
        }
        #endregion

        #region Protected Methods
        protected override void ShowAlert(string message)
        {
            // הפעולה מקבלת את ההודעה ומציגה אותה למשתמש
            message = fbd.GetErrorMessage(message);
            General.ToastMake(message);
        }
        protected override void OnCompleteRegister(Task task)
        {
            // הפעולה מקבלת את המשימה ולא מחזירה שום ערך
            IsBusy = false;
            if (task.IsCompletedSuccessfully)
            {
                SaveToPreferences();
                SaveToSecureStorage();
                OnAuthenticationComplete?.Invoke(this, true);
            }
            else
                if (task.Exception != null)
                {
                    Name = string.Empty;
                    Email = string.Empty;
                    Password = string.Empty;
                    Exception ex = task.Exception.InnerException ?? task.Exception;
                    Console.WriteLine(ex.Message);
                    ShowAlert(ex.Message);
                    OnAuthenticationComplete?.Invoke(this, false);

                }
        }
        protected override void OnCompleteLogin(Task task)
        {
            // הפעולה מקבלת את המשימה ולא מחזירה שום ערך
            IsBusy = false;
            if (task.IsCompletedSuccessfully)
            {
                SaveToPreferences();
                SaveToSecureStorage();
                OnAuthenticationComplete?.Invoke(this, true);
            }
            else
            {
                General.ToastMake(Strings.LoginFailedError);
                Name = string.Empty;
                Email = string.Empty;
                Password = string.Empty;
                OnAuthenticationComplete?.Invoke(this, false);

            }
        }
        protected override void OnCompleteBiometric(Task task)
        {
            // הפעולה מקבלת את המשימה ולא מחזירה שום ערך
            IsBusy = false;
            if (task.IsCompletedSuccessfully)
                OnAuthenticationComplete?.Invoke(this, true);
            else
            {
                General.ToastMake(Strings.LoginFailedError);
                OnAuthenticationComplete?.Invoke(this, false);
            }
        }
        protected override void OnCompleteResetPassword(Task task)
        {
            // הפעולה מקבלת את המשימה ולא מחזירה שום ערך
            IsBusy = false;
            if (!task.IsCompletedSuccessfully)
            {
                if (task.Exception != null)
                {
                    Exception ex = task.Exception.InnerException ?? task.Exception;
                    Console.WriteLine(ex.Message);
                }
                General.ToastMake(Strings.ResetPasswordFailed);
            }
        }
        protected override void SaveToSecureStorage()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            SecureStorage.SetAsync(Keys.LastEmailKey, Email);
            SecureStorage.SetAsync(Keys.LastPasswordKey, Password);
        }
        #endregion
    }
}
