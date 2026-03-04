using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Plugin.CloudFirestore;
using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Platforms.Android
{
    [Service]
    public class DeleteFbDocsService : Service
    {
        // service class that deleted fb documents after a day
        #region Fields
        private bool isRunning = true;
        private readonly FbData fbd = new();
        #endregion

        #region Public Methods
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent? intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            ThreadStart threadStart = new(DeleteFbDocs);
            Thread thread = new(threadStart);
            thread.Start();
            return base.OnStartCommand(intent, flags, startId);
        }
        public override IBinder? OnBind(Intent? intent)
        {
            // Not used
            return null;
        }
        public override void OnDestroy()
        {
            isRunning = false;
            base.OnDestroy();
        }
        #endregion

        #region Private Methods
        private void DeleteFbDocs()
        {
            while (isRunning)
            {
                fbd.GetDocumentsWhereLessThan(Keys.GamesCollection, nameof(GameModel.Created), DateTime.Now.AddDays(-1), OnComplete);
                Thread.Sleep(Keys.OneHourInMilliseconds);
            }
            StopSelf();
        }
        private void OnComplete(IQuerySnapshot qs)
        {
            foreach (IDocumentSnapshot doc in qs.Documents)
                fbd.DeleteDocument(Keys.GamesCollection, doc.Id, (task) => { });
        }
        #endregion
    }
}
