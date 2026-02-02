using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Java.Lang;
using Plugin.CloudFirestore;
using Plugin.Maui.Biometric;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    public class Game : GameModel
    {
        private readonly CardsSet myCards;
        public override string OpponentName => IsHostUser ? GuestName : HostName;
        protected override GameStatus Status => _status;

        internal Game()
        {
            Console.WriteLine(DeckCards.Count);
            myCards = new CardsSet(full: false)
            {
                SingleSelect = false
            };
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });

        }

        internal Game(bool value)
        {
            Console.WriteLine(DeckCards.Count);
            myCards = new CardsSet(full: false)
            {
                SingleSelect = false
            };
            HostName = fbd.DisplayName;
            Created = DateTime.Now;
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
            UpdateStatus();
        }

        private void OnMessageReceived(long timeLeft)
        {
            TimeLeft = timeLeft != Keys.FinishedSignal ? double.Round(timeLeft / 1000, 1).ToString() : Strings.TimeUp;
            TimeLeftChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void UpdateStatus()
        {
            _status.CurrentStatus = IsHostUser && IsHostTurn || !IsHostUser && !IsHostTurn ?
                GameStatus.Statuses.Play : GameStatus.Statuses.Wait;
        }

        public override void SetDocument(Action<System.Threading.Tasks.Task> OnComplete)
        {
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);

        }
        public override void UpdateGuestUser(Action<Task> OnComplete)
        {
            IsFull = true;
            GuestName = MyName;
            UpdateFbJoinGame(OnComplete);
        }

        protected override void UpdateFbJoinGame(Action<Task> OnComplete)
        {
            Dictionary<string, object> dict = new()
            {
                { nameof(IsFull), IsFull },
                { nameof(GuestName), GuestName }
            };
            fbd.UpdateFields(Keys.GamesCollection, Id, dict, OnComplete);
        }

        public override void UpdateFbInGame(Action<Task> OnComplete)
        {
            Dictionary<string, object> dict = new()
            {
                { nameof(DeckCards), DeckCards },
            };
            fbd.UpdateFields(Keys.GamesCollection, Id, dict, OnComplete);
        }

        public override void AddSnapshotListener()
        {
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, Id, OnChange);
        }

        public override void RemoveSnapshotListener()
        {
            ilr?.Remove();
            DeleteDocument(OnComplete);
        }

        protected override void OnComplete(Task task)
        {
            if (task.IsCompletedSuccessfully)
                GameDeleted?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnChange(IDocumentSnapshot? snapshot, System.Exception? error)
        {
            Game? updatedGame = snapshot?.ToObject<Game>();
            if (updatedGame != null)
            {
                IsFull = updatedGame.IsFull;
                GuestName = updatedGame.GuestName;
                IsHostTurn = updatedGame.IsHostTurn;
                DeckCards = updatedGame.DeckCards;
                QueenTableCards = updatedGame.QueenTableCards;
                GameChanged?.Invoke(this, EventArgs.Empty);
                UpdateStatus();
                if (_status.CurrentStatus == GameStatus.Statuses.Play)
                {
                    WeakReferenceMessenger.Default.Send(new AppMessage<TimerSettings>(timerSettings));
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new AppMessage<bool>(true));
                    TimeLeft = string.Empty;
                    TimeLeftChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.Navigation.PopAsync();
                    Toast.Make(Strings.GameCanceled, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                });

            }
        }
        public override Card TakeCard()
        {
            Card card = DeckCards.TakeCard();
            if (!card.IsEmpty)
            {
                card = myCards.Add(card);
            }
            return card;
        }
        public override void DeleteDocument(Action<Task> OnComplete)
        {
            fbd.DeleteDocument(Keys.GamesCollection, Id, OnComplete);
        }

        public void SelectCard(Card card)
        {
            myCards.SelectCard(card);
        }

        public List<Card> ThrowCard()
        {
            List<Card> card = myCards.ThrowCard();
            if (card.Count >= 1)
            {
                OpenedCard = card[0];
                //isSelectedMatch = false;

            }
            return card;
        }
    }
}
