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
        public readonly CardsSet myCards;
        public override string OpponentName => IsHostUser ? GuestName : HostName;
        protected override GameStatus Status => _status;

        internal Game()
        {
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

        protected void OnMessageReceived(long timeLeft)
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
                { nameof(OpenedCard), OpenedCard },
                { nameof(QueenTableCards), QueenTableCards },
                { nameof(IsHostTurn), IsHostTurn   }
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
                OpenedCard = updatedGame.OpenedCard;
                QueenTableCards = updatedGame.QueenTableCards;
                GameChanged?.Invoke(this, EventArgs.Empty);
                UpdateStatus();
                if (_status.CurrentStatus == GameStatus.Statuses.Play)
                {
                    if (TimeLeft == string.Empty && DeckCards.Count < 66)
                    {
                        WeakReferenceMessenger.Default.Send(new AppMessage<TimerSettings>(timerSettings));
                    }
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
            if (_status.CurrentStatus == GameStatus.Statuses.Play)
                myCards.SelectCard(card);
        }

        public List<Card> ThrowCard()
        {
            List<Card> card = myCards.ThrowCard();
            if (card.Count >= 1)
            {
                OpenedCard = card[0];
            }
            return card;
        }

        public bool CanEndTurn()
        {
            return _status.CurrentStatus == GameStatus.Statuses.Play && myCards.CardsDeck.Count < 5 && DeckCards.CardsDeck.Count < 66;
        }

        public bool CanThrowCards()
        {
            bool canThrow = true;
            if (_status.CurrentStatus != GameStatus.Statuses.Play)
                canThrow = false;
            else if (myCards.SelectedCards.Count == 0)
                canThrow = false;
            else if (myCards.SelectedCards.Count == 2)
            {
                if (myCards.SelectedCards[0].Type != Strings.number || myCards.SelectedCards[1].Type != Strings.number)
                    canThrow = false;
                if (myCards.SelectedCards[0].Value != myCards.SelectedCards[1].Value)
                    canThrow = false;
            }
            else if (myCards.SelectedCards.Count == 3)
            {
                if (myCards.SelectedCards[0].Type == Strings.number && myCards.SelectedCards[1].Type == Strings.number && myCards.SelectedCards[2].Type == Strings.number)
                    if (myCards.SelectedCards[0].Value == myCards.SelectedCards[1].Value && myCards.SelectedCards[1].Value == myCards.SelectedCards[2].Value)
                        return canThrow;


                if (TryGetEquation(myCards.SelectedCards, out string? equation))
                {
                    Console.WriteLine(equation);
                }
                else
                    canThrow = false;
            }
            else if (myCards.SelectedCards.Count == 4)
            {
                if (myCards.SelectedCards[0].Type == Strings.number && myCards.SelectedCards[1].Type == Strings.number && myCards.SelectedCards[2].Type == Strings.number && myCards.SelectedCards[3].Type == Strings.number)
                    if (myCards.SelectedCards[0].Value == myCards.SelectedCards[1].Value && myCards.SelectedCards[1].Value == myCards.SelectedCards[2].Value && myCards.SelectedCards[2].Value == myCards.SelectedCards[3].Value)
                        return canThrow;


                if (TryGetEquation(myCards.SelectedCards, out string? equation))
                {
                    Console.WriteLine(equation);
                }
                else
                    canThrow = false;
            }
            else if (myCards.SelectedCards.Count == 5)
            {
                if (myCards.SelectedCards[0].Type == Strings.number && myCards.SelectedCards[1].Type == Strings.number && myCards.SelectedCards[2].Type == Strings.number && myCards.SelectedCards[3].Type == Strings.number && myCards.SelectedCards[4].Type == Strings.number)
                    if (myCards.SelectedCards[0].Value == myCards.SelectedCards[1].Value && myCards.SelectedCards[1].Value == myCards.SelectedCards[2].Value && myCards.SelectedCards[2].Value == myCards.SelectedCards[3].Value && myCards.SelectedCards[3].Value == myCards.SelectedCards[4].Value)
                        return canThrow;


                if (TryGetEquation(myCards.SelectedCards, out string? equation))
                {
                    Console.WriteLine(equation);
                }
                else
                    canThrow = false;
            }
            return canThrow;
        }
        public static bool TryGetEquation(List<Card> cards, out string? equation)
        {
            equation = null;

            // If any card is negative, immediately reject
            foreach (Card card in cards)
            {
                if (card.Value < 0)
                    return false;
            }


            // Try each card as the result
            for (int i = 0; i < cards.Count; i++)
            {
                int result = cards[i].Value;

                // Collect the other cards
                List<int> numbers = [];
                for (int j = 0; j < cards.Count; j++)
                {
                    if (j != i)
                        numbers.Add(cards[j].Value);
                }

                // Start checking combinations
                if (CheckSequence(numbers, 1, numbers[0], numbers[0].ToString(), result, out string expr))
                {
                    equation = expr + " = " + result;
                    return true;
                }
            }

            return false;
        }
        private static bool CheckSequence(List<int> numbers, int index, int current, string expr, int target, out string finalExpr)
        {
            finalExpr = "";

            if (index >= numbers.Count)
            {
                if (current == target)
                {
                    finalExpr = expr;
                    return true;
                }
                return false;
            }

            int next = numbers[index];

            if (CheckSequence(numbers, index + 1, current + next, expr + " + " + next, target, out string addExpr))
            {
                finalExpr = addExpr;
                return true;
            }

            if (CheckSequence(numbers, index + 1, current - next, expr + " - " + next, target, out string subExpr))
            {
                finalExpr = subExpr;
                return true;
            }

            return false;
        }

        public void InitializeQueens()
        {
            for (int i = 0; i < 12; i++)
            {
                bool found = false;
                int number = 0;
                while (!found)
                {
                    found = true;
                    number = random.Next(0, 12);
                    for (int j = 0; j < QueenTableCards.Count; j++)
                    {
                        if (QueenTableCards[j].Value == number) found = false;
                    }
                }
                QueenTableCards.Add(new Card(Strings.queen, number));
            }
        }

        public void ChangeTurn()
        {
            IsHostTurn = !IsHostTurn;
            UpdateStatus();
        }
    }
}
