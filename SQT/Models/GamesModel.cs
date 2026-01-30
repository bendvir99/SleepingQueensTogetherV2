using Java.Interop;
using Plugin.CloudFirestore;
using SleepingQueensTogether.ModelsLogic;
using System.Collections.ObjectModel;

namespace SleepingQueensTogether.Models
{
    public abstract class GamesModel
    {
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;
        protected Game? currentGame;

        public bool IsBusy { get; set; }
        public Game? CurrentGame { get => currentGame; set => currentGame = value; }
        public ObservableCollection<Game>? GamesList { get; set; } = [];

        public EventHandler<Game>? GameAdded;
        public EventHandler? GamesChanged;
        public abstract void RemoveSnapshotListener();
        public abstract void AddSnapshotListener();
        public abstract void AddGame();
        protected abstract void OnGameDeleted(object? sender, EventArgs e);
        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IQuerySnapshot snapshot, System.Exception error);
        protected abstract void OnComplete(IQuerySnapshot qs);

    }
}
