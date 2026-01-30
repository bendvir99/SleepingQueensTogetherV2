namespace SleepingQueensTogether.Models
{
    public class GoogleAuthService
    {
        private const string ClientId = "705683475351-n3bts7a4d5p8ou1ajojlto8catie1mpj.apps.googleusercontent.com";
        private const string RedirectUri = "com.googleusercontent.apps.705683475351-n3bts7a4d5p8ou1ajojlto8catie1mpj:/oauth2redirect";

        public async Task<string> GetGoogleIdTokenAsync()
        {
            string? idToken = null; // nullable string

            string authUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                             "?client_id=" + ClientId +
                             "&redirect_uri=" + RedirectUri +
                             "&response_type=id_token" +
                             "&scope=openid%20email%20profile" +
                             "&nonce=random_nonce";

            Uri callbackUrl = new Uri(RedirectUri);

            WebAuthenticatorResult result = await WebAuthenticator.Default.AuthenticateAsync(
                new Uri(authUrl),
                callbackUrl
            );

            if (result.Properties.TryGetValue("id_token", out idToken))
            {
                return idToken!;
            }

            // fallback: search any property containing "token"
            foreach (KeyValuePair<string, string> kv in result.Properties)
            {
                if (kv.Key.ToLower().Contains("token"))
                {
                    return kv.Value;
                }
            }

            throw new Exception("Google sign-in succeeded but ID token not found.");
        }
    }
}
