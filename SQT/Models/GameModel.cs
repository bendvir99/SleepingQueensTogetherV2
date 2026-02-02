using Java.Lang;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    public abstract class GameModel
    {
        protected static FbData fbd = new();
        protected Random random = new();
        protected IListenerRegistration? ilr;
        protected TimerSettings timerSettings = new(Keys.TimerTotalTime, Keys.TimerInterval);
        protected abstract GameStatus Status { get; }
        protected GameStatus _status = new();
        [Ignored]
        public string StatusMessage => Status.StatusMessage;
        [Ignored]
        public EventHandler? GameChanged;
        [Ignored]
        public EventHandler? GameDeleted;
        [Ignored]
        public EventHandler? TimeLeftChanged;
        [Ignored]
        public string Id { get; set; } = string.Empty;
        [Ignored]
        public string MyName { get; set; } = fbd.DisplayName;
        [Ignored]
        public bool IsHostUser { get; set; }
        [Ignored]
        public abstract string OpponentName { get; }
        [Ignored]
        public string TimeLeft { get; protected set; } = string.Empty;

        //public Card[] Cards = [new(), new(), new(), new(), new()];
        //[Ignored]
        //public List<Card> DeckCards = [new Card("Number", 1), new Card("Number", 2), new Card("Number", 3), new Card("Number", 4), new Card("Number", 5), new Card("Number", 6), new Card("Number", 7), new Card("Number", 8), new Card("Number", 9), new Card("Number", 10), new Card("Number", 1), new Card("Number", 2), new Card("Number", 3), new Card("Number", 4), new Card("Number", 5), new Card("Number", 6), new Card("Number", 7), new Card("Number", 8), new Card("Number", 9), new Card("Number", 10), new Card("Number", 1), new Card("Number", 2), new Card("Number", 3), new Card("Number", 4), new Card("Number", 5), new Card("Number", 6), new Card("Number", 7), new Card("Number", 8), new Card("Number", 9), new Card("Number", 10), new Card("Number", 1), new Card("Number", 2), new Card("Number", 3), new Card("Number", 4), new Card("Number", 5), new Card("Number", 6), new Card("Number", 7), new Card("Number", 8), new Card("Number", 9), new Card("Number", 10), new Card("King", 1), new Card("King", 2), new Card("King", 3), new Card("King", 4), new Card("King", 5), new Card("King", 6), new Card("King", 7), new Card("Knight", 1), new Card("Knight", 2), new Card("Knight", 3), new Card("Knight", 4), new Card("Jester", 1), new Card("Jester", 1), new Card("Jester", 1), new Card("Jester", 1), new Card("Jester", 1), new Card("SleepingPotion", 1), new Card("SleepingPotion", 1), new Card("SleepingPotion", 1), new Card("SleepingPotion", 1), new Card("Wand", 1), new Card("Wand", 1), new Card("Wand", 1), new Card("Dragon", 1), new Card("Dragon", 1), new Card("Dragon", 1)];
        public List<Card> QueenTableCards = new(12);
        public string HostName { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public CardsSet DeckCards { get; set; } = new CardsSet(full: true);
        public DateTime Created { get; set; }
        public Card OpenedCard { get; set; } = new Card();
        public bool IsFull { get; set; }
        public bool IsHostTurn { get; set; } = false;
        public bool IsSelectedMatch { get; set; }
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void RemoveSnapshotListener();
        public abstract void AddSnapshotListener();
        public abstract void DeleteDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void UpdateGuestUser(Action<Task> OnComplete);
        //public abstract void InitializeCards();
        public abstract Card TakeCard();
        public abstract void UpdateFbInGame(Action<Task> OnComplete);
        protected abstract void UpdateStatus();
        protected abstract void UpdateFbJoinGame(Action<Task> OnComplete);
        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IDocumentSnapshot? snapshot, System.Exception? error);
    }
}
