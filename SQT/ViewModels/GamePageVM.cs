using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;
        private readonly StackLayout stkMyCards;
        public string Timeleft => Strings.TimeLeft + game.TimeLeft;
        public string MyName => game.MyName;
        public string StatusMessage => game.StatusMessage;
        public string OpponentName => game.OpponentName;
        public string Total => $"{Strings.TotalQueens}\n{Strings.TotalPoints}";
        public bool GameStarted => game.DeckCards.Count < 66;
        public bool OpenedCardVisible => game.OpenedCard.Type != Strings.empty;
        //public string[] QueenCardImages => [.. Enumerable.Range(0, 12).Select(i => GetCardImage(i))];
        public bool CanStartGame => CanStart();
        public ICommand ChangeTurnCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand SelectCardCommand { get; }
        public ICommand ThrowSelectedCardCommand { get; }

        public ImageSource OpenedCardImage
        {
            get
            {
                CardView cardView = new();
                cardView.SetCardSource(game.OpenedCard.Type, game.OpenedCard.Value);
                return cardView.Source;
            }
        }
        public GamePageVM(Game game, StackLayout stkMyCards)
        {
            game.GameChanged += OnGameChanged;
            game.TimeLeftChanged += OnTimeLeftChanged;
            ChangeTurnCommand = new Command(ChangeTurn);
            StartGameCommand = new Command(StartGame);
            ThrowSelectedCardCommand = new Command(ThrowSelectedCard, CanThrowCards);
            SelectCardCommand = new Command<SelectCardEventArgs>(SelectCard);
            this.game = game;
            this.stkMyCards = stkMyCards;
            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnComplete);
            }
        }

        private void ThrowSelectedCard()
        {
            List<Card> card = game.ThrowCard();
            if (card.Count >= 1)
            {
                for (int i = 0; i < card.Count; i++)
                {
                    stkMyCards.Children.RemoveAt(card[i].Index);
                }
               (ThrowSelectedCardCommand as Command)?.ChangeCanExecute();
            }
            game.UpdateFbInGame(OnCompleteUpdate);
            OnPropertyChanged(nameof(OpenedCardImage));
            OnPropertyChanged(nameof(OpenedCardVisible));
        }

        private void SelectCard(SelectCardEventArgs args)
        {
            if (args.SelectedCard != null && args.SelectedCardView != null)
            {
                game.SelectCard(args.SelectedCard);
                args.SelectedCardView.Margin = args.SelectedCard.Margin;
                (ThrowSelectedCardCommand as Command)?.ChangeCanExecute();
            }
        }

        private void OnTimeLeftChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Timeleft));
        }

        private void StartGame()
        {
            for (int i = 0; i < 5; i++)
            {
                TakePackageCard();
            }
            game.InitializeQueens();
            game.UpdateFbInGame(OnCompleteUpdate);
            OnPropertyChanged(nameof(CanStartGame));
            OnPropertyChanged(nameof(GameStarted));
        }

        private void TakePackageCard()
        {
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
            else
                Toast.Make("No more cards", ToastDuration.Long, 20).Show();
        }
        private void OnCompleteUpdate(Task task)
        {
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.UpdateErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
        }
        private bool CanStart()
        {
            return !game.IsHostUser && game.DeckCards.Count == 66;
        }
        private bool CanThrowCards()
        {
            return game.CanThrowCards();
        }
        private void ChangeTurn()
        {
            game.IsHostTurn = !game.IsHostTurn;
            Dictionary<string, object> dict = new()

            {
                { nameof(game.IsHostTurn), game.IsHostTurn }
            };
            FbData fbd = new();
            fbd.UpdateFields(Keys.GamesCollection, game.Id, dict, OnComplete);
            OnPropertyChanged(nameof(StatusMessage));
        }
        //private string GetCardImage(int index)
        //{
        //    if (game.QueenTableCards.Count == 0)
        //        return Strings.greencard;
        //    if (!game.QueenTableCards[index].IsAwaken)
        //    {
        //        return game.QueenTableCards[index].ImageCard;
        //    }
        //    else
        //        return game.QueenTableCards[index].BackImage;
        //}
        private void OnGameChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(OpponentName));
            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(OpenedCardImage));
            OnPropertyChanged(nameof(OpenedCardVisible));
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
            if (!task.IsCompletedSuccessfully)
                Toast.Make(Strings.JoinGameErr, CommunityToolkit.Maui.Core.ToastDuration.Long, 14);

        }
        public void AddSnapshotListener()
        {
            game.AddSnapshotListener();
        }

        public void RemoveSnapshotListener()
        {
            game.RemoveSnapshotListener();
        }
    }
}

