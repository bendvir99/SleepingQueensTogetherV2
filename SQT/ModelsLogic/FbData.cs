using Plugin.CloudFirestore;
using Plugin.Maui.Biometric;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    public partial class FbData : FbDataModel
    {

        public override async void CreateUserWithEmailAndPasswordAsync(string email, string password, string name, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.CreateUserWithEmailAndPasswordAsync(email, password, name).ContinueWith(OnComplete);
        }
        public override async void SignInWithEmailAndPasswordAsync(string email, string password, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(OnComplete);
        }
        public override async void ResetPasswordWithEmail(string email, Action<System.Threading.Tasks.Task> OnComplete)
        {
            await facl.ResetEmailPasswordAsync(email).ContinueWith(OnComplete);
        }
        //public override async void SignInWithGoogleAsync(Action<System.Threading.Tasks.Task> OnComplete)
        //{
        //    GoogleAuthService google = new GoogleAuthService();
        //    string idToken = await google.GetGoogleIdTokenAsync();

        //    if (string.IsNullOrEmpty(idToken))
        //    {
        //        throw new Exception("No Google ID token returned.");
        //    }

        //    // Correct call for FirebaseAuthentication.net
        //    Firebase.Auth.FirebaseAuthLink user = await facl.SignInWithOAuthAsync("google.com", idToken);

        //    OnComplete(Task.CompletedTask);
        //}

        public override string GetErrorMessage(string message)
        {
            if (message.Contains(Strings.InvalidEmail))
            {
                return Strings.RegisterFailedInvalidEmail;
            }
            else if (message.Contains(Strings.EmailExists))
            {
                return (Strings.RegisterFailedEmailExists);
            }
            else if (message.Contains(Strings.WeakPassword))
            {
                return (Strings.RegisterFailedWeakPassword);
            }
            else
            {
                return (Strings.RegisterUnknownError);
            }
        }

        public override string SetDocument(object obj, string collectonName, string id, Action<System.Threading.Tasks.Task> OnComplete)
        {
            IDocumentReference dr = string.IsNullOrEmpty(id) ? fdb.Collection(collectonName).Document() : fdb.Collection(collectonName).Document(id);
            dr.SetAsync(obj).ContinueWith(OnComplete);
            return dr.Id;
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, Plugin.CloudFirestore.QuerySnapshotHandler OnChange)
        {
            ICollectionReference cr = fdb.Collection(collectonName);
            return cr.AddSnapshotListener(OnChange);
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, string id, Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            IDocumentReference cr = fdb.Collection(collectonName).Document(id);
            return cr.AddSnapshotListener(OnChange);
        }
        public override async void GetDocumentsWhereEqualTo(string collectonName, string fName, object fValue, Action<IQuerySnapshot> OnComplete)
        {
            ICollectionReference cr = fdb.Collection(collectonName);
            IQuerySnapshot qs = await cr.WhereEqualsTo(fName, fValue).GetAsync();
            OnComplete(qs);
        }
        public override async void UpdateFields(string collectonName, string id, Dictionary<string, object> dict, Action<Task> OnComplete)
        {
            IDocumentReference dr = fdb.Collection(collectonName).Document(id);
            await dr.UpdateAsync(dict).ContinueWith(OnComplete);
        }

        public override async void DeleteDocument(string collectonName, string id, Action<Task> OnComplete)
        {
            IDocumentReference dr = fdb.Collection(collectonName).Document(id);
            await dr.DeleteAsync().ContinueWith(OnComplete);
        }

        public override async void GetDocumentsWhereLessThan(string collectonName, string fName, object fValue, Action<IQuerySnapshot> OnComplete)
        {
            ICollectionReference cr = fdb.Collection(collectonName);
            IQuerySnapshot qs = await cr.WhereLessThan(fName, fValue).GetAsync();
            OnComplete(qs);
        }
    }
}
