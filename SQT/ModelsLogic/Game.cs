using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.CloudFirestore;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    public class Game : GameModel
    {
        // במחלקה הזאת מתנהל כל הפעולות של המשחק
        #region Fields
        protected override GameStatus Status => _status;
        #endregion

        #region Properties
        public override string OpponentName => IsHostUser ? GuestName : HostName;
        #endregion

        #region Constructors
        internal Game()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }
        internal Game(bool value)
        {
            // הפעולה מקבלת בוליאן רק כדי להבדיל בין הפעולה הבונה של היוצר של המשחק והשחקן שנכנס. הפעולה לא מחזירה שום ערכים
            HostName = fbd.DisplayName;
            Created = DateTime.Now;
            WeakReferenceMessenger.Default.Register<AppMessage<long>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
            UpdateStatus();
        }
        #endregion

        #region Public Methods
        public override void SetDocument(Action<System.Threading.Tasks.Task> OnComplete)
        {
            // הפעולה מקבלת את הפעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            Id = fbd.SetDocument(this, Keys.GamesCollection, Id, OnComplete);

        }
        public override void UpdateGuestUser(Action<Task> OnComplete)
        {
            // הפעולה מקבלת את הפעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            IsFull = true;
            GuestName = MyName;
            UpdateFbJoinGame(OnComplete);
        }
        public override void UpdateFbInGame(Action<Task> OnComplete)
        {
            // הפעולה מקבלת את הפעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            Dictionary<string, object> dict = new()
            {
                { nameof(DeckCards), DeckCards },
                { nameof(OpenedCard), OpenedCard },
                { nameof(QueenTableCards), QueenTableCards },
                { nameof(IsHostTurn), IsHostTurn },
                { nameof(IllegalMove), IllegalMove },
                { nameof(UpdateMessage), UpdateMessage },
                { nameof(QueenCards), QueenCards},
                { nameof(myCards), myCards },
                { nameof(KnightPlaced), KnightPlaced },
                { nameof(PotionPlaced), PotionPlaced },
                { nameof(CanClickQueen), CanClickQueen },
                { nameof(RemoveQueen), RemoveQueen },
                { nameof(RemoveQueenIndex), RemoveQueenIndex }
            };
            fbd.UpdateFields(Keys.GamesCollection, Id, dict, OnComplete);
        }
        public override void AddSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערכים
            ilr = fbd.AddSnapshotListener(Keys.GamesCollection, Id, OnChange);
        }
        public override void RemoveSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערכים
            ilr?.Remove();
            DeleteDocument(OnComplete);
        }
        public override Card TakeCard()
        {
            // הפעולה לא מקבלת שום פרמטרים ומחזירה קלף שנלקח מהחבילה
            Card card = DeckCards.TakeCard();
            if (!card.IsEmpty)
                card = myCards.Add(card);
            return card;
        }
        public override void DeleteDocument(Action<Task> OnComplete)
        {
            // הפעולה מקבלת את הפעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            WeakReferenceMessenger.Default.Unregister<AppMessage<long>>(this);
            fbd.DeleteDocument(Keys.GamesCollection, Id, OnComplete);
        }
        public override void SelectCard(Card card)
        {
            // הפעולה מקבלת קלף שנבחר ומסמנת אותו כבחור או מבטלת את הבחירה בו. הפעולה לא מחזירה שום ערכים
            if (_status.CurrentStatus == GameStatus.Statuses.Play && myCards.CardsDeck.Count == 5 && !CanPickQueen)
            {
                if (KnightPlaced && CanClickQueen && card.Type == Strings.dragon) myCards.SelectCard(card);
                else if (PotionPlaced && CanClickQueen && card.Type == Strings.wand) myCards.SelectCard(card);
                else if (!KnightPlaced && !PotionPlaced && !CanClickQueen) myCards.SelectCard(card);
            }
        }
        public override List<Card> ThrowCard(out string? equation)
        {
            // הפעולה מקבלת את המשוואה של הקלפים שנזרקו (אם יש) ומחזירה את הקלפים שנזרקו
            equation = null;
            List<Card> card = [];
            if (CanThrowCards(out string? equationFinal, out int final))
            {
                if (final != -1)
                {
                    for (int i = 0; i < myCards.SelectedCards.Count; i++)
                        if (myCards.SelectedCards[i].Value == final)
                        {
                            OpenedCard = myCards.SelectedCards[i];
                            equation = equationFinal;
                        }
                }
                else
                    OpenedCard = myCards.SelectedCards[0];
                card = myCards.ThrowCard();
            }
            return card;
        }
        public override void InitializeQueens()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערכים.
            for (int i = 0; i < 12; i++)
            {
                bool found = false;
                int number = 0;
                while (!found)
                {
                    found = true;
                    number = random.Next(0, 12);
                    for (int j = 0; j < QueenTableCards.Count; j++)
                        if (QueenTableCards[j].Value == number)
                            found = false;
                }
                QueenTableCards.Add(new Card(Strings.queen, number));
            }
        }
        public override void ChangeTurn()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערכים. 
            IsHostTurn = !IsHostTurn;
        }
        public override void UseCardPower(string type)
        {
            // הפעולה מקבלת את סוג הקלף שהשתמשו בו ומפעילה את הכוח שלו. הפעולה לא מחזירה שום ערכים
            if (type == Strings.king)
                CanPickQueen = true;
            else if (type == Strings.joker)
            {
                int num = random.Next(0, 2);
                if (num == 0)
                {
                    Toast.Make(Strings.JokerYou, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    UpdateMessage = Strings.JokerOpponent;
                    CanPickQueen = true;
                    UpdateFbInGame(OnCompleteUpdate);
                }
                else
                {
                    Toast.Make(Strings.JokerOpponent, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    UpdateMessage = Strings.JokerYou;
                    ChangeTurn();
                    UpdateFbInGame(OnCompleteUpdate);
                }
            }
            else if (type == Strings.knight && OpponentQueenCards.Count > 0)
            {
                CanClickQueen = true;
                for (int i = 0; i < OpponentCards.Count; i++)
                    if (OpponentCards[i].Type == Strings.dragon)
                    {
                        KnightPlaced = true;
                        ChangeTurn();
                    }
            }
            else if (type == Strings.sleepingpotion && OpponentQueenCards.Count > 0)
            {
                CanClickQueen = true;
                for (int i = 0; i < OpponentCards.Count; i++)
                    if (OpponentCards[i].Type == Strings.wand)
                    {
                        PotionPlaced = true;
                        ChangeTurn();
                    }
            }
            else if (type == Strings.dragon || type == Strings.wand)
            {
                CanClickQueen = false;
            }
        }
        public override void TakeQueenCard(int index)
        {
            // הפעולה מקבלת את האינדקס של קלף המלכה שנלקח. הפעולה לא מחזירה שום ערכים
            QueenCards.Add(QueenTableCards[index]);
            QueenTableCards[index].IsAwaken = true;
            QueensCount++;
            QueenPoints += QueenTableCards[index].QueenValue;
            CanPickQueen = false;
            if (myCards.CardsDeck.Count == 5)
                ChangeTurn();
        }

        public override void ClickQueenCard(string index)
        {
            // הפעולה מקבלת את האינדקס של קלף המלכה שנלחץ. הפעולה לא מחזירה שום ערכים
            int i = int.Parse(index);
            if (CanClickQueen && OpponentQueenCards.Count > i && _status.CurrentStatus == GameStatus.Statuses.Play)
            {
                if (OpenedCard.Type == Strings.knight)
                {
                    QueenCards.Add(OpponentQueenCards[i]);
                    QueenPoints += OpponentQueenCards[i].QueenValue;
                    QueensCount++;
                }
                else
                    for (int j = 0; j < QueenTableCards.Count; j++)
                        if (QueenTableCards[j].Value == OpponentQueenCards[i].Value)
                            QueenTableCards[j].IsAwaken = false;
                if (QueensCount >= 5)
                {
                    UpdateMessage = Strings.OpponentWon + QueensCount + Strings.Queens;
                    SendToast?.Invoke(this, new ToastMessageEventArgs(Strings.YouWon + QueensCount + Strings.Queens));
                    EndGame?.Invoke(this, EventArgs.Empty);
                }
                else if (QueenPoints >= 50)
                {
                    UpdateMessage = Strings.OpponentWon + QueenPoints + Strings.Points;
                    SendToast?.Invoke(this, new ToastMessageEventArgs(Strings.YouWon + QueenPoints + Strings.Points));
                    EndGame?.Invoke(this, EventArgs.Empty);
                }
                OpponentQueenCards.Remove(OpponentQueenCards[i]);
                RemoveQueen = true;
                RemoveQueenIndex = i;
                CanClickQueen = false;
                UpdateFbInGame(OnCompleteUpdate);
                RemoveQueen = false;
                RemoveQueenIndex = -1;

            }
        }
        public override void TakeQueenCard(string index)
        {
            // הפעולה מקבלת את האינדקס של קלף המלכה שנלקח. הפעולה לא מחזירה שום ערכים
            int i = int.Parse(index);
            if (CanPickQueen && !QueenTableCards[i].IsAwaken && _status.CurrentStatus == GameStatus.Statuses.Play)
            {
                TakeQueenCard(i);
                if (QueensCount >= 5)
                {
                    UpdateMessage = Strings.OpponentWon + QueensCount + Strings.Queens;
                    SendToast?.Invoke(this, new ToastMessageEventArgs(Strings.YouWon + QueensCount + Strings.Queens));
                    EndGame?.Invoke(this, EventArgs.Empty);
                }
                else if (QueenPoints >= 50)
                {
                    UpdateMessage = Strings.OpponentWon + QueenPoints + Strings.Points;
                    SendToast?.Invoke(this, new ToastMessageEventArgs(Strings.YouWon + QueenPoints + Strings.Points));
                    EndGame?.Invoke(this, EventArgs.Empty);
                }
                UpdateFbInGame(OnCompleteUpdate);
            }
        }

        public override void EndTurn()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערכים.
            KnightPlaced = false;
            PotionPlaced = false;
            ChangeTurn();
            UpdateFbInGame(OnCompleteUpdate);
        }
        #endregion

        #region Protected Methods
        protected override void OnMessageReceived(long timeLeft)
        {
            // הפעולה מקבלת את הזמן שנותר לתור של השחקן הנוכחי. הפעולה לא מחזירה שום ערכים
            if (timeLeft == Keys.FinishedSignal)
            { 
                if (_status.CurrentStatus == GameStatus.Statuses.Play)
                {
                    CanPickQueen = false;
                    if (!KnightPlaced && !PotionPlaced) CanClickQueen = false;
                    KnightPlaced = false;
                    PotionPlaced = false;
                    UpdateMessage = Strings.TimeUp;
                    ChangeTurn();
                    UpdateFbInGame(OnCompleteUpdate);
                    Toast.Make(Strings.TimeUp, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    TimesUp?.Invoke(this, EventArgs.Empty);
                }
            }
            else
                TimeLeft = double.Round(timeLeft / 1000, 1).ToString();
            TimeLeftChanged?.Invoke(this, EventArgs.Empty);
        }
        protected override void UpdateStatus()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערכים. 
            _status.CurrentStatus = IsHostUser && IsHostTurn || !IsHostUser && !IsHostTurn ?
                GameStatus.Statuses.Play : GameStatus.Statuses.Wait;
        }
        protected override void UpdateFbJoinGame(Action<Task> OnComplete)
        {
            // הפעולה מקבלת את הפעולה שפועלת לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            Dictionary<string, object> dict = new()
            {
                { nameof(IsFull), IsFull },
                { nameof(GuestName), GuestName }
            };
            fbd.UpdateFields(Keys.GamesCollection, Id, dict, OnComplete);
        }
        protected override void OnComplete(Task task)
        {
            // הפעולה מקבלת את המשימה שהושלמה לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.DeleteErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
        }
        protected override void OnCompleteUpdate(Task task)
        {
            // הפעולה מקבלת את המשימה שהושלמה לאחר שהפעולה הזאת מסיימת. הפעולה לא מחזירה שום ערכים
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.UpdateErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
        }
        protected override void OnChange(IDocumentSnapshot? snapshot, System.Exception? error)
        {
            // הפעולה מקבלת את השינויים שנעשו במשחק ואת השגיאה אם ישנה לאחר שהמשחק עודכן. הפעולה לא מחזירה שום ערכים
            Game? updatedGame = snapshot?.ToObject<Game>();
            if (updatedGame != null)
            {
                IsFull = updatedGame.IsFull;
                GuestName = updatedGame.GuestName;
                IsHostTurn = updatedGame.IsHostTurn;
                DeckCards = updatedGame.DeckCards;
                OpenedCard = updatedGame.OpenedCard;
                QueenTableCards = updatedGame.QueenTableCards;
                IllegalMove = updatedGame.IllegalMove;
                UpdateMessage = updatedGame.UpdateMessage;
                KnightPlaced = updatedGame.KnightPlaced;
                PotionPlaced = updatedGame.PotionPlaced;
                CanClickQueen = updatedGame.CanClickQueen;
                if (_status.CurrentStatus == GameStatus.Statuses.Wait)
                {
                    RemoveQueen = updatedGame.RemoveQueen;
                    RemoveQueenIndex = updatedGame.RemoveQueenIndex;
                    if (UpdateMessage != string.Empty)
                    {
                        Toast.Make(UpdateMessage, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                        if (UpdateMessage == Strings.JokerYou)
                            CanPickQueen = true;
                    }
                    if (RemoveQueen)
                    {
                        QueenPoints -= QueenCards[RemoveQueenIndex].QueenValue;
                        QueensCount--;
                        QueenCards.Remove(QueenCards[RemoveQueenIndex]);
                    }
                    if (IllegalMove) Toast.Make(Strings.IllegalMove, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    UpdateMessage = string.Empty;
                    IllegalMove = false;
                    RemoveQueen = false;
                    RemoveQueenIndex = -1;
                    OpponentQueenCards = updatedGame.QueenCards;
                    OpponentCards = updatedGame.myCards.CardsDeck;
                }
                if (_status.CurrentStatus == GameStatus.Statuses.Play)
                {
                    RemoveQueen = false;
                    RemoveQueenIndex = -1;
                    if (DeckCards.Count == 56 && myCards.CardsDeck.Count == 5) OpponentCards = updatedGame.myCards.CardsDeck;
                    UpdateMessage = string.Empty;
                    IllegalMove = false;
                }
                UpdateStatus();
                if (_status.CurrentStatus == GameStatus.Statuses.Play)
                {
                    if (TimeLeft == string.Empty && DeckCards.Count < 66)
                        WeakReferenceMessenger.Default.Send(new AppMessage<TimerSettings>(timerSettings));
                }
                else
                {
                    WeakReferenceMessenger.Default.Send(new AppMessage<bool>(true));
                    TimeLeft = string.Empty;
                    TimeLeftChanged?.Invoke(this, EventArgs.Empty);
                }
                GameChanged?.Invoke(this, EventArgs.Empty);
            }
            else
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Shell.Current.Navigation.PopAsync();
                });
        }
        protected override bool CanThrowCards(out string? equationFinal, out int finalNumber)
        {
            // הפעולה מקבלת את המשוואה של הקלפים שנזרקו (אם יש) ואת המספר הסופי של הקלפים שנזרקו (אם יש) ומחזירה בוליאן שאומר אם אפשר לזרוק את הקלפים שנבחרו או לא
            finalNumber = -1;
            equationFinal = null;
            bool canThrow = true;
            if (_status.CurrentStatus != GameStatus.Statuses.Play)
                canThrow = false;
            else if (myCards.SelectedCards.Count == 0)
                canThrow = false;
            else if (myCards.SelectedCards.Count == 2)
                if ((myCards.SelectedCards[0].Type == Strings.number &&
                    myCards.SelectedCards[1].Type == Strings.number) &&
                    (myCards.SelectedCards[0].Value == myCards.SelectedCards[1].Value))
                {
                    equationFinal = Strings.Double + myCards.SelectedCards[0].Value;
                    finalNumber = myCards.SelectedCards[0].Value;
                    canThrow = true;
                }
                else
                    canThrow = false;
            else if (myCards.SelectedCards.Count == 3)
            {
                if ((myCards.SelectedCards[0].Type == Strings.number &&
                    myCards.SelectedCards[1].Type == Strings.number) &&
                    (myCards.SelectedCards[0].Value == myCards.SelectedCards[1].Value) &&
                    (myCards.SelectedCards[1].Value == myCards.SelectedCards[2].Value))
                {
                    equationFinal = Strings.Triple + myCards.SelectedCards[0].Value;
                    finalNumber = myCards.SelectedCards[0].Value;
                    canThrow = true;
                }
                else if (TryGetEquation(myCards.SelectedCards, out string? equation, out int final))
                {
                    equationFinal = equation;
                    finalNumber = final;
                    canThrow = true;
                }
                else
                    canThrow = false;
            }
            else if (myCards.SelectedCards.Count == 4)
            {
                if ((myCards.SelectedCards[0].Type == Strings.number &&
                    myCards.SelectedCards[1].Type == Strings.number) &&
                    (myCards.SelectedCards[0].Value == myCards.SelectedCards[1].Value) &&
                    (myCards.SelectedCards[1].Value == myCards.SelectedCards[2].Value) &&
                    (myCards.SelectedCards[2].Value == myCards.SelectedCards[3].Value))
                {
                    equationFinal = Strings.Quadruple + myCards.SelectedCards[0].Value;
                    finalNumber = myCards.SelectedCards[0].Value;
                    canThrow = true;
                }
                else if (TryGetEquation(myCards.SelectedCards, out string? equation, out int final))
                {
                    equationFinal = equation;
                    finalNumber = final;
                    canThrow = true;
                }
                else
                    canThrow = false;
            }
            else if (myCards.SelectedCards.Count == 5)
            {
                if ((myCards.SelectedCards[0].Type == Strings.number &&
                    myCards.SelectedCards[1].Type == Strings.number) &&
                    (myCards.SelectedCards[0].Value == myCards.SelectedCards[1].Value) &&
                    (myCards.SelectedCards[1].Value == myCards.SelectedCards[2].Value) &&
                    (myCards.SelectedCards[2].Value == myCards.SelectedCards[3].Value) &&
                    (myCards.SelectedCards[3].Value == myCards.SelectedCards[4].Value))
                {
                    equationFinal = Strings.Quintuple + myCards.SelectedCards[0].Value;
                    finalNumber = myCards.SelectedCards[0].Value;
                    canThrow = true;
                }
                else if (TryGetEquation(myCards.SelectedCards, out string? equation, out int final))
                {
                    equationFinal = equation;
                    finalNumber = final;
                    canThrow = true;
                }
                else
                    canThrow = false;
            }
            return canThrow;
        }
        protected static bool TryGetEquation(List<Card> cards, out string? equation, out int final)
        {
            // הפעולה מקבלת את הקלפים שנבחרו ומחזירה את המשוואה של הקלפים האלו (אם יש) ואת המספר הסופי של הקלפים האלו (אם יש) ומחזירה בוליאן שאומר אם אפשר ליצור משוואה עם הקלפים האלו או לא
            bool found = false;
            bool flag = true;
            final = -1;
            equation = null;
            foreach (Card card in cards)
                if (card.Value < 0)
                    flag = false;
            if (flag)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    int result = cards[i].Value;
                    List<int> numbers = [];
                    for (int j = 0; j < cards.Count; j++)
                        if (j != i)
                            numbers.Add(cards[j].Value);

                    if (CheckSequence(numbers, 1, numbers[0], numbers[0].ToString(), result, out string expr))
                    {
                        equation = expr + Strings.EqualsText + result;
                        final = result;
                        found = true;
                    }
                }
            }
            return found;
        }
        protected static bool CheckSequence(List<int> numbers, int index, int current, string expr, int target, out string finalExpr)
        {
            // הפעולה מקבלת את המספרים שנשארו, האינדקס של המספר הבא, התוצאה הנוכחית, המשוואה הנוכחית, המספר היעד ומחזירה את המשוואה הסופית אם נמצאה משוואה שמגיעה למספר היעד עם המספרים האלו ומחזירה בוליאן שאומר אם נמצאה משוואה כזו או לא
            bool found = false;
            finalExpr = string.Empty;
            if (index >= numbers.Count)
            {
                if (current == target)
                {
                    finalExpr = expr;
                    found = true;
                }
            }
            else
            {
                int next = numbers[index];
                if (CheckSequence(numbers, index + 1, current + next, expr + Strings.Plus + next, target, out string addExpr))
                {
                    finalExpr = addExpr;
                    found = true;
                }
                else if (CheckSequence(numbers, index + 1, current - next, expr + Strings.Minus + next, target, out string subExpr))
                {
                    finalExpr = subExpr;
                    found = true;
                }
            }
            return found;
        }
        #endregion
    }
}
