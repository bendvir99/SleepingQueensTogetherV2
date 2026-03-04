using Plugin.CloudFirestore;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    class Games() : GamesModel
    {
        // במחלקה הזאת מתנהל כל הפעולות של המשחקים 
        #region Public Methods
        public override void AddGame()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            IsBusy = true;
            currentGame = new(true)
            {
                IsHostUser = true
            };
            currentGame.SetDocument(OnComplete);
        }
        public override void AddSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, OnChange!);
        }
        public override void RemoveSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            ilr?.Remove();
        }
        #endregion

        #region Protected Methods
        protected override void OnChange(IQuerySnapshot snapshot, System.Exception error)
        {
            // הפעולה מקבלת את הסנפשוט ושגיאה ולא מחזירה שום ערך
            fbd.GetDocumentsWhereEqualTo(Keys.GamesCollection, nameof(GameModel.IsFull), false, OnComplete);
        }
        protected override void OnComplete(IQuerySnapshot qs)
        {
            // הפעולה מקבלת את הסנפשוט ולא מחזירה שום ערך
            GamesList!.Clear();
            foreach (IDocumentSnapshot ds in qs.Documents)
            {
                Game? game = ds.ToObject<Game>();
                if (game != null)
                {
                    game.Id = ds.Id;
                    GamesList.Add(game);
                }
            }
            GamesChanged?.Invoke(this, EventArgs.Empty);
        }
        protected override void OnComplete(Task task)
        {
            // הפעולה מקבלת את המשימה ולא מחזירה שום ערך
            IsBusy = false;
            GameAdded?.Invoke(this, currentGame!);
        }
        #endregion
    }
}
