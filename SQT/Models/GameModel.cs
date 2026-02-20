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
        [Ignored]
        public bool CanPickQueen { get; set; } = false;
        [Ignored]
        public List<Card> OpponentQueenCards = new(5);
        [Ignored]
        public int QueensCount { get; set; } = 0;
        [Ignored]
        public int QueenPoints { get; set; } = 0;
        public List<Card> QueenCards = new(5);
        public string Equation { get; set; } = string.Empty;
        public List<Card> QueenTableCards = new(12);
        public string HostName { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public CardsSet DeckCards { get; set; } = new CardsSet(full: true);
        public DateTime Created { get; set; }
        public Card OpenedCard { get; set; } = new Card();
        public bool IsFull { get; set; }
        public bool IsHostTurn { get; set; } = false;
        public bool IllegalMove { get; set; } = false;
        public bool IsSelectedMatch { get; set; }
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void RemoveSnapshotListener();
        public abstract void AddSnapshotListener();
        public abstract void DeleteDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void UpdateGuestUser(Action<Task> OnComplete);
        public abstract Card TakeCard();
        public abstract void UpdateFbInGame(Action<Task> OnComplete);
        protected abstract void UpdateStatus();
        protected abstract void UpdateFbJoinGame(Action<Task> OnComplete);
        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IDocumentSnapshot? snapshot, System.Exception? error);
    }
}
