using CommunityToolkit.Maui.Alerts;
using Java.Lang;
using Plugin.CloudFirestore;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    class Games : GamesModel
    {
        public override void AddGame()
        {
            IsBusy = true;
            currentGame = new(true)
            {
                IsHostUser = true
            };
            currentGame.GameDeleted += OnGameDeleted;
            currentGame.SetDocument(OnComplete);
        }
        protected override void OnGameDeleted(object? sender, EventArgs e)
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Toast.Make(Strings.GameCanceled, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
            });
        }
        protected override void OnComplete(Task task)
        {
            IsBusy = false;
            GameAdded?.Invoke(this, currentGame!);
        }
        public Games()
        {

        }
        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, OnChange!);
        }
        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
        }
        protected override void OnChange(IQuerySnapshot snapshot, System.Exception error)
        {
            fbd.GetDocumentsWhereEqualTo(Keys.GamesCollection, nameof(GameModel.IsFull), false, OnComplete);
        }

        protected override void OnComplete(IQuerySnapshot qs)
        {
            GamesList!.Clear();
            //if(qs.Documents.Count() >0)
            //{
            //    IDocumentSnapshot ds = qs.Documents.FirstOrDefault()!;
            //    Game? game = ds.ToObject<Game>();
            //}
            foreach (IDocumentSnapshot ds in qs.Documents)
            {
                Game? game = ds.ToObject<Game>();
                Console.WriteLine(game?.DeckCards.Count);
                if (game != null)
                {
                    game.Id = ds.Id;
                    GamesList.Add(game);
                }
            }
            GamesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
