using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    public abstract class GameModel
    {
        // מחלקת המודל של המשחק בה יש את כל הנתונים של המשחק
        #region Fields
        protected static FbData fbd = new();
        protected Random random = new();
        protected IListenerRegistration? ilr;
        protected TimerSettings timerSettings = new(Keys.TimerTotalTime, Keys.TimerInterval);
        protected abstract GameStatus Status { get; }
        protected GameStatus _status = new();
        #endregion

        #region Events
        [Ignored]
        public EventHandler? GameChanged;
        [Ignored]
        public EventHandler? TimeLeftChanged;
        [Ignored]
        public EventHandler? TimesUp;
        [Ignored]
        public EventHandler? EndGame;
        [Ignored]
        public EventHandler<ToastMessageEventArgs>? SendToast;
        #endregion

        #region Properties
        [Ignored]
        public string StatusMessage => Status.StatusMessage;
        [Ignored]
        public string Id { get; set; } = string.Empty;
        [Ignored]
        public string MyName { get; } = fbd.DisplayName;
        [Ignored]
        public string TimeLeft { get; protected set; } = string.Empty;
        [Ignored]
        public bool IsHostUser { get; set; }
        [Ignored]
        public abstract string OpponentName { get; }
        [Ignored]
        public bool CanPickQueen { get; protected set; } = false;
        [Ignored]
        public bool GameStarted => DeckCards.Count < 66;
        [Ignored]
        public List<Card> OpponentQueenCards = new(5);
        [Ignored]
        public List<Card> OpponentCards = new(5);
        [Ignored]
        public int QueensCount { get; protected set; } = 0;
        [Ignored]
        public int QueenPoints { get; protected set; } = 0;
        public string UpdateMessage { get; set; } = string.Empty;
        public string HostName { get; protected set; } = string.Empty;
        public string GuestName { get; protected set; } = string.Empty;
        public List<Card> QueenCards = new(5);
        public List<Card> QueenTableCards = new(12);
        public CardsSet DeckCards { get; protected set; } = new CardsSet(full: true);
        public CardsSet myCards { get; protected set; } = new CardsSet(full: false);
        public DateTime Created { get; protected set; }
        public Card OpenedCard { get; protected set; } = new Card();
        public bool IsFull { get; protected set; }
        public bool IsHostTurn { get; protected set; } = false;
        public bool IllegalMove { get; set; } = false;
        public bool KnightPlaced { get; set; } = false;
        public bool PotionPlaced { get; set; } = false;
        public bool CanClickQueen { get; set; } = false;
        public bool RemoveQueen { get; protected set; } = false;
        public int RemoveQueenIndex { get; protected set; } = -1;
        #endregion

        #region Public Methods
        public abstract void SetDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void RemoveSnapshotListener();
        public abstract void ClickQueenCard(string index);
        public abstract void TakeQueenCard(string index);
        public abstract void UseCardPower(string type);
        public abstract void TakeQueenCard(int index);
        public abstract void EndTurn();
        public abstract void InitializeQueens();
        public abstract void ChangeTurn();
        public abstract void SelectCard(Card card);
        public abstract void AddSnapshotListener();
        public abstract void DeleteDocument(Action<System.Threading.Tasks.Task> OnComplete);
        public abstract void UpdateGuestUser(Action<Task> OnComplete);
        public abstract void UpdateFbInGame(Action<Task> OnComplete);
        public abstract Card TakeCard();
        public abstract List<Card> ThrowCard(out string? equation);
        #endregion

        #region Protected Methods
        protected abstract void UpdateStatus();
        protected abstract void OnMessageReceived(long timeLeft);
        protected abstract void OnCompleteUpdate(Task task);
        protected abstract void UpdateFbJoinGame(Action<Task> OnComplete);
        protected abstract void OnComplete(Task task);
        protected abstract void OnChange(IDocumentSnapshot? snapshot, System.Exception? error);
        protected abstract bool CanThrowCards(out string? equationFinal, out int finalNumber);
        #endregion
    }
}
