using Plugin.CloudFirestore;
using SleepingQueensTogether.ModelsLogic;
using System.Collections.ObjectModel;

namespace SleepingQueensTogether.Models
{
    public abstract class GamesModel
    {
        // מחלקת המודל של המשחקים שבה יש את כל המידע על רשימת המשחק והמשחק הנוכחי.
        #region Fields
        protected FbData fbd = new();
        protected IListenerRegistration? ilr;
        protected Game? currentGame;
        #endregion

        #region Events
        public EventHandler<Game>? GameAdded;
        public EventHandler? GamesChanged;
        #endregion

        #region Properties
        public bool IsBusy { get; protected set; }
        public Game? CurrentGame { get => currentGame; set => currentGame = value; }
        public ObservableCollection<Game>? GamesList { get; protected set; } = [];
        #endregion

        #region Public Methods
        public abstract void RemoveSnapshotListener();
        public abstract void AddSnapshotListener();
        public abstract void AddGame();
        #endregion

        #region Private Methods
        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IQuerySnapshot snapshot, System.Exception error);
        protected abstract void OnComplete(IQuerySnapshot qs);
        #endregion
    }
}
