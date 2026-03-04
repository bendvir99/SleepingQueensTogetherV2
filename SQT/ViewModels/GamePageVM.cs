using CommunityToolkit.Maui.Alerts;
using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        // הפעולה שמקשר את הלוגיקה של המשחק לדף המשחק
        #region Fields
        private readonly Game game;
        private readonly StackLayout stkMyCards;
        #endregion

        #region ICommands
        public ICommand StartGameCommand { get; }
        public ICommand SelectCardCommand { get; }
        public ICommand ThrowSelectedCardCommand { get; }
        public ICommand EndTurnCommand { get; }
        public ICommand TakeQueenCardCommand { get; }
        public ICommand ClickQueenCardCommand { get; }
        #endregion

        #region Properties
        public string Timeleft => Strings.TimeLeft + game.TimeLeft;
        public string MyName => game.MyName;
        public string StatusMessage => game.StatusMessage;
        public string OpponentName => game.OpponentName;
        public string Total => $"{Strings.TotalQueens}" + game.QueensCount + $"\n{Strings.TotalPoints}" + game.QueenPoints;
        public bool GameStarted => game.GameStarted;
        public bool OpenedCardVisible => game.OpenedCard.Type != Strings.empty;
        public ImageSource?[] QueenTableCardImages => [.. Enumerable.Range(0, 12).Select(i => GetCardImage(i, 1))];
        public ImageSource?[] QueenCardImages => [.. Enumerable.Range(0, 5).Select(i => GetCardImage(i, 2))];
        public ImageSource?[] OpponentQueenCardImages => [.. Enumerable.Range(0, 5).Select(i => GetCardImage(i, 3))];
        public bool CanStartGame => CanStart();
        public ImageSource OpenedCardImage
        {
            get
            {
                CardView cardView = new();
                cardView.SetCardSource(game.OpenedCard.Type, game.OpenedCard.Value);
                return cardView.Source;
            }
        }
        #endregion

        #region Constructor
        public GamePageVM(Game game, StackLayout stkMyCards)
        {
            // הפעולה מקבלת את המשחק ואת הסטאק ליאוט של הקלפים שהשחקן מחזיק ביד. הפעולה לא מחזירה שום ערך
            game.GameChanged += OnGameChanged;
            game.TimeLeftChanged += OnTimeLeftChanged;
            game.TimesUp += OnTimesUp;
            game.SendToast += OnSendToast;
            game.EndGame += OnEndGame;
            StartGameCommand = new Command(StartGame);
            EndTurnCommand = new Command(EndTurn);
            ThrowSelectedCardCommand = new Command(ThrowSelectedCard);
            SelectCardCommand = new Command<SelectCardEventArgs>(SelectCard);
            TakeQueenCardCommand = new Command<string>(TakeQueenCard);
            ClickQueenCardCommand = new Command<string>(ClickQueenCard);
            this.game = game;
            this.stkMyCards = stkMyCards;
            if (!game.IsHostUser)
                game.UpdateGuestUser(OnComplete);
        }
        #endregion

        #region Public Methods
        public void AddSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            game.AddSnapshotListener();
        }
        public void RemoveSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            game.RemoveSnapshotListener();
        }
        #endregion

        #region Private Methods
        private void OnSendToast(object? sender, ToastMessageEventArgs e)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת הנתונים של האיוונט. הפעולה לא מחזירה שום ערך
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Toast.Make(e.message, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
            });
        }
        private void ClickQueenCard(string index)
        {
            // הפעולה מקבלת את האינדקס של הקלף. הפעולה לא מחזירה שום ערך
            game.ClickQueenCard(index);
            OnPropertyChanged(nameof(OpponentQueenCardImages));
            OnPropertyChanged(nameof(QueenCardImages));
            OnPropertyChanged(nameof(Total));
        }
        private async static void OnEndGame(object? sender, EventArgs e)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת הנתונים של האיוונט. הפעולה לא מחזירה שום ערך
            await Task.Delay(3000);
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PopAsync();
            });
        }
        private void TakeQueenCard(string index)
        {
            // הפעולה מקבלת את האינדקס של הקלף ולא מחזירה שום ערך
            game.TakeQueenCard(index);
            OnPropertyChanged(nameof(QueenTableCardImages));
            OnPropertyChanged(nameof(QueenCardImages));
            OnPropertyChanged(nameof(Total));
        }
        private void EndTurn()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (!game.CanPickQueen && game.StatusMessage == Strings.PlayMessage && game.myCards.CardsDeck.Count < 5 && !game.CanClickQueen)
            {
                while (game.myCards.CardsDeck.Count < 5)
                    TakePackageCard();
                game.EndTurn();
            }
            OnPropertyChanged(nameof(game.IsHostTurn));
        }
        private void ThrowSelectedCard()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (game.myCards.CardsDeck.Count == 5 && game.StatusMessage == Strings.PlayMessage && !game.CanPickQueen)
            {
                List<Card> card = game.ThrowCard(out string? equation);
                if (card.Count >= 1)
                {
                    for (int i = 0; i < card.Count; i++)
                    {
                        stkMyCards.Children.RemoveAt(card[i].Index);
                        if (card[i].Type != Strings.number) game.UseCardPower(card[i].Type);
                    }
                    if (equation != null)
                    {
                        Toast.Make(equation, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                        game.UpdateMessage = equation;
                    }
                }
                else
                {
                    for (int i = 0; i < stkMyCards.Children.Count; i++)
                        if (stkMyCards.Children[i].Margin != new Thickness(0))
                            if (stkMyCards.Children[i] is ImageButton imagebutton && imagebutton.Command != null)
                                imagebutton.Command.Execute(imagebutton.CommandParameter);
                    game.ChangeTurn();
                    game.IllegalMove = true;
                    if (!game.KnightPlaced && !game.PotionPlaced) game.CanClickQueen = false;
                    game.KnightPlaced = false;
                    game.PotionPlaced = false;
                    Toast.Make(Strings.IllegalMove, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                }
                game.UpdateFbInGame(OnCompleteUpdate);
                OnPropertyChanged(nameof(OpenedCardImage));
                OnPropertyChanged(nameof(game.IsHostTurn));
                OnPropertyChanged(nameof(OpenedCardVisible));
            }
        }
        private void SelectCard(SelectCardEventArgs args)
        {
            // הפעולה מקבלת את הנתונים של האיוונט ולא מחזירה שום ערך
            if (args.SelectedCard != null && args.SelectedCardView != null)
            {
                game.SelectCard(args.SelectedCard);
                args.SelectedCardView.Margin = args.SelectedCard.Margin;
            }
        }
        private void OnTimeLeftChanged(object? sender, EventArgs e)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת הנתונים של האיוונט. הפעולה לא מחזירה שום ערך
            OnPropertyChanged(nameof(Timeleft));
        }
        private void OnTimesUp(object? sender, EventArgs e)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת הנתונים של האיוונט. הפעולה לא מחזירה שום ערך
            for (int i = 0; i < stkMyCards.Children.Count; i++)
                if (stkMyCards.Children[i].Margin != new Thickness(0))
                    if (stkMyCards.Children[i] is ImageButton imagebutton && imagebutton.Command != null)
                        imagebutton.Command.Execute(imagebutton.CommandParameter);
            while (game.myCards.CardsDeck.Count < 5)
                TakePackageCard();
        }
        private void StartGame()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            for (int i = 0; i < 5; i++)
                TakePackageCard();
            game.InitializeQueens();
            game.UpdateFbInGame(OnCompleteUpdate);
            OnPropertyChanged(nameof(CanStartGame));
            OnPropertyChanged(nameof(GameStarted));
            OnPropertyChanged(nameof(QueenCardImages));
        }
        private void TakePackageCard()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            Card card = game.TakeCard();
            CardView cardView = new();
            cardView.SetCardSource(card.Type, card.Value);
            cardView.Margin = card.Margin;
            cardView.Index = card.Index;
            cardView.WidthRequest = 70;
            cardView.HeightRequest = 100;
            if (card != null)
            {
                stkMyCards.Children.Add(cardView);
                SelectCardEventArgs scea = new() { SelectedCard = card, SelectedCardView = cardView};
                cardView.CommandParameter = scea;
                cardView.Command = SelectCardCommand;
            }
        }
        private void OnCompleteUpdate(Task task)
        {
            // הפעולה מקבלת את המשימה ולא מחזירה שום ערך
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.UpdateErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
        }
        private bool CanStart()
        {
            // הפעולה לא מקבלת שום פרמטרים ומחזירה האם אפשר להתחיל את המשחק או לא
            return !game.IsHostUser && game.DeckCards.Count == 66;
        }
        private ImageSource? GetCardImage(int index, int type)
        {
            // הפעולה מקבלת את האינדקס של הקלף ואיזה סוג קלף ומחזירה את התמונה של הקלף
            ImageSource? source = Strings.greencard;
            if (type == 1)
            {
                if (index >= game.QueenTableCards.Count)
                    source = null;
                else if (game.QueenTableCards[index].IsAwaken)
                    source = null;
            }
            else if (type == 2)
            {
                if (index >= game.QueenCards.Count)
                    source = null;
                else if (game.QueenCards.Count == 0)
                    source = null;
                else if (game.QueenCards[index] != null)
                {
                    CardView cardView = new();
                    cardView.SetCardSource(game.QueenCards[index].Type, game.QueenCards[index].Value);
                    source = cardView.Source;
                }
                else
                    source = null;
            }
            else
            {
                if (game.OpponentQueenCards.Count == 0)
                    source = null;
                else if (index >= game.OpponentQueenCards.Count)
                    source = null;
                else if (game.OpponentQueenCards[index] != null)
                {
                    CardView cardView = new();
                    cardView.SetCardSource(game.OpponentQueenCards[index].Type, game.OpponentQueenCards[index].Value);
                    source = cardView.Source;
                }
                else
                    source = null;
            }
            return source;
        }
        private void OnGameChanged(object? sender, EventArgs e)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת הנתונים של האיוונט. הפעולה לא מחזירה שום ערך
            OnPropertyChanged(nameof(OpponentName));
            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(OpenedCardImage));
            OnPropertyChanged(nameof(OpenedCardVisible));
            OnPropertyChanged(nameof(QueenCardImages));
            OnPropertyChanged(nameof(QueenTableCardImages));
            OnPropertyChanged(nameof(OpponentQueenCardImages));
            OnPropertyChanged(nameof(game.IsHostTurn));
            OnPropertyChanged(nameof(Total));
            if (game.DeckCards.Count == 61 && game.IsHostUser)
            {
                for (int i= 0; i < 5; i++)
                    TakePackageCard();
                game.UpdateFbInGame(OnCompleteUpdate);
                OnPropertyChanged(nameof(GameStarted));
            }
        }
        private void OnComplete(Task task)
        {
            // הפעולה מקבלת את המשימה ולא מחזירה שום ערך
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.JoinGameErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14);
        }
        #endregion
    }
}

