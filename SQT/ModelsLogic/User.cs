using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    internal class User : UserModel
    {
        private bool canUseBiometrics = false;
        public override void Register()
        {
            IsBusy = true;
            CurrentAction = Actions.Register;
            fbd.CreateUserWithEmailAndPasswordAsync(Email, Password, Name,OnCompleteRegister);
        }
        public override void Login()
        {
            IsBusy = true;
            fbd.SignInWithEmailAndPasswordAsync(Email, Password, OnCompleteLogin);
        }
        public override void CheckBiometricAvailability()
        {
            _ = Task.Run(async () =>
            {
                string? savedEmail = await SecureStorage.GetAsync(Keys.LastEmailKey);
                string? savedPassword = await SecureStorage.GetAsync(Keys.LastPasswordKey);

                bool biometricAvailable = await CrossFingerprint.Current.IsAvailableAsync(true);

                canUseBiometrics = biometricAvailable &&
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
            {
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
        }




        public override void ResetPassword(string email)
        {
            IsBusy = true;

            fbd.ResetPasswordWithEmail(email, OnCompleteResetPassword);
        }

        protected override void ShowAlert(string message)
        {
            message = fbd.GetErrorMessage(message);
            General.ToastMake(message);
        }

        protected override void OnCompleteRegister(Task task)
        {
            IsBusy = false;
            if (task.IsCompletedSuccessfully)
            {
                SaveToPreferences();
                SaveToSecureStorage();
                OnAuthenticationComplete?.Invoke(this, true);
            }
            else
            {
                if (task.Exception != null)
                {
                    Exception ex = task.Exception.InnerException ?? task.Exception;

                    ShowAlert(ex.Message);
                    OnAuthenticationComplete?.Invoke(this, false);

                }
                Name = string.Empty;
                Email = string.Empty;
                Password = string.Empty;
            }
        }
        protected override void OnCompleteLogin(Task task)
        {
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
                OnAuthenticationComplete?.Invoke(this, false);
                Name = string.Empty;
                Email = string.Empty;
                Password = string.Empty;

            }
        }
        protected override void OnCompleteBiometric(Task task)
        {
            IsBusy = false;
            if (task.IsCompletedSuccessfully)
            {
                OnAuthenticationComplete?.Invoke(this, true);
            }
            else
            {
                General.ToastMake(Strings.LoginFailedError);
                OnAuthenticationComplete?.Invoke(this, false);
            }
        }
        protected override void OnCompleteResetPassword(Task task)
        {
            IsBusy = false;
            if (!task.IsCompletedSuccessfully)
            {
                if (task.Exception != null)
                {
                    Exception ex = task.Exception.InnerException ?? task.Exception;
                    Console.WriteLine(ex);
                }
                General.ToastMake(Strings.ResetPasswordFailed);
            }
        }
        protected override void SaveToSecureStorage()
        {
            SecureStorage.SetAsync(Keys.LastEmailKey, Email);
            SecureStorage.SetAsync(Keys.LastPasswordKey, Password);

        }

        public override void SaveToPreferences()
        {
            Preferences.Set(Keys.UsernameKey, Name);
            Preferences.Set(Keys.GmailKey, Email);
            Preferences.Set(Keys.PasswordKey, Password);
            Preferences.Set(Keys.RememberMeKey, RememberMe);
        }
        public override bool IsValidRegister()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email) && !IsBusy;
        }
        public override bool IsValidLogin()
        {
            return !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(Email) && !IsBusy;
        }
        public override bool IsValidBiometric()
        {
            return canUseBiometrics && !IsBusy;
        }


    }
}
