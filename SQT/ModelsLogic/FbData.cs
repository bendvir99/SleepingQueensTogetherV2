using Plugin.CloudFirestore;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    public partial class FbData : FbDataModel
    {
        // במחלקה הזאת מתנהל כל הפעולות של הפיירבייס וכל מה שאפשר לעשות איתו
        #region Public Methods
        public override async void CreateUserWithEmailAndPasswordAsync(string email, string password, string name, Action<System.Threading.Tasks.Task> OnComplete)
        {
            // הפעולה מקבלת אימייל, סיסמא, שם ופעולה שפועלת לאחר שהפעולה מסיימת. הפעולה לא מחזירה שום ערכים
            await facl.CreateUserWithEmailAndPasswordAsync(email, password, name).ContinueWith(OnComplete);
        }
        public override async void SignInWithEmailAndPasswordAsync(string email, string password, Action<System.Threading.Tasks.Task> OnComplete)
        {
            // הפעולה מקבלת אימייל סיסמא ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            await facl.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(OnComplete);
        }
        public override async void ResetPasswordWithEmail(string email, Action<System.Threading.Tasks.Task> OnComplete)
        {
            // הפעולה מקבלת אימייל ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערך
            await facl.ResetEmailPasswordAsync(email).ContinueWith(OnComplete);
        }
        public override string GetErrorMessage(string message)
        {
            // הפעולה מקבלת הודעה ומחזירה את השגיאה מתוך ההודעה.
            string error = Strings.RegisterUnknownError;
            if (message.Contains(Strings.InvalidEmail))
                error = Strings.RegisterFailedInvalidEmail;
            else if (message.Contains(Strings.EmailExists))
                error = (Strings.RegisterFailedEmailExists);
            else if (message.Contains(Strings.WeakPassword))
                error = (Strings.RegisterFailedWeakPassword);
            return error;
        }
        public override string SetDocument(object obj, string collectonName, string id, Action<System.Threading.Tasks.Task> OnComplete)
        {
            // הפעולה מקבלת אובייקט ושם של קולקשיין, וגם איי דיי ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה מחזירה את המסמך
            IDocumentReference dr = string.IsNullOrEmpty(id) ? fdb.Collection(collectonName).Document() : fdb.Collection(collectonName).Document(id);
            dr.SetAsync(obj).ContinueWith(OnComplete);
            return dr.Id;
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, Plugin.CloudFirestore.QuerySnapshotHandler OnChange)
        {
            // הפעולה מקבלת שם של קולקשיין ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה מחזירה את ההאזנה לשינויים במסמך
            ICollectionReference cr = fdb.Collection(collectonName);
            return cr.AddSnapshotListener(OnChange);
        }
        public override IListenerRegistration AddSnapshotListener(string collectonName, string id, Plugin.CloudFirestore.DocumentSnapshotHandler OnChange)
        {
            // הפעולה מקבלת שם של קולקשיין, איי דיי ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה מחזירה את ההאזנה לשינויים במסמך
            IDocumentReference cr = fdb.Collection(collectonName).Document(id);
            return cr.AddSnapshotListener(OnChange);
        }
        public override async void GetDocumentsWhereEqualTo(string collectonName, string fName, object fValue, Action<IQuerySnapshot> OnComplete)
        {
            // הפעולה מקבלת שם של קולקשיין, שם של שדה, ערך של שדה ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה מחזירה את המסמכים שבהם השדה שווה לערך
            ICollectionReference cr = fdb.Collection(collectonName);
            IQuerySnapshot qs = await cr.WhereEqualsTo(fName, fValue).GetAsync();
            OnComplete(qs);
        }
        public override async void UpdateFields(string collectonName, string id, Dictionary<string, object> dict, Action<Task> OnComplete)
        {
            // הפעולה מקבלת שם של קולקשיין, איי דיי, מילון של שדות וערכים ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            IDocumentReference dr = fdb.Collection(collectonName).Document(id);
            await dr.UpdateAsync(dict).ContinueWith(OnComplete);
        }
        public override async void DeleteDocument(string collectonName, string id, Action<Task> OnComplete)
        {
            // הפעולה מקבלת שם של קולקשיין, איי דיי ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            IDocumentReference dr = fdb.Collection(collectonName).Document(id);
            await dr.DeleteAsync().ContinueWith(OnComplete);
        }
        public override async void GetDocumentsWhereLessThan(string collectonName, string fName, object fValue, Action<IQuerySnapshot> OnComplete)
        {
            // הפעולה מקבלת שם של קולקשיין, שם של שדה, ערך של שדה ופעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה מחזירה את המסמכים שבהם השדה קטן לערך
            ICollectionReference cr = fdb.Collection(collectonName);
            IQuerySnapshot qs = await cr.WhereLessThan(fName, fValue).GetAsync();
            OnComplete(qs);
        }
        #endregion
    }
}
