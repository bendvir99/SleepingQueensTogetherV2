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
        public ImageSource[] QueenCardImages => [.. Enumerable.Range(0, 12).Select(i => GetCardImage(i))];
        public bool CanStartGame => CanStart();
        public ICommand StartGameCommand { get; }
        public ICommand SelectCardCommand { get; }
        public ICommand ThrowSelectedCardCommand { get; }
        public ICommand EndTurnCommand { get; }

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
            StartGameCommand = new Command(StartGame);
            EndTurnCommand = new Command(EndTurn, CanEndTurn);
            ThrowSelectedCardCommand = new Command(ThrowSelectedCard);
            SelectCardCommand = new Command<SelectCardEventArgs>(SelectCard);
            this.game = game;
            this.stkMyCards = stkMyCards;
            if (!game.IsHostUser)
                game.UpdateGuestUser(OnComplete);
        }

        private bool CanEndTurn()
        {
            return game.CanEndTurn();
        }

        private void EndTurn()
        {
            for (int i = 0; i < (5 - game.myCards.CardsDeck.Count);)
                TakePackageCard();
            game.ChangeTurn();
            game.UpdateFbInGame(OnCompleteUpdate);
            OnPropertyChanged(nameof(game.IsHostTurn));
            (EndTurnCommand as Command)?.ChangeCanExecute();
        }

        private void ThrowSelectedCard()
        {
            if (game.myCards.CardsDeck.Count == 5 && game.StatusMessage == Strings.PlayMessage)
            {
                List<Card> card = game.ThrowCard(out string? equation);
                if (card.Count >= 1)
                {
                    for (int i = 0; i < card.Count; i++)
                        stkMyCards.Children.RemoveAt(card[i].Index);
                    (EndTurnCommand as Command)?.ChangeCanExecute();
                    if (equation != null)
                    {
                        Toast.Make(equation, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                        game.Equation = equation;
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
                    Toast.Make(Strings.IllegalMove, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                }
                game.UpdateFbInGame(OnCompleteUpdate);
                OnPropertyChanged(nameof(OpenedCardImage));
                OnPropertyChanged(nameof(game.IsHostTurn));
                OnPropertyChanged(nameof(OpenedCardVisible));
                game.IllegalMove = false;
                game.Equation = string.Empty;
            }
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
                TakePackageCard();
            game.InitializeQueens();
            game.UpdateFbInGame(OnCompleteUpdate);
            OnPropertyChanged(nameof(CanStartGame));
            OnPropertyChanged(nameof(GameStarted));
            OnPropertyChanged(nameof(QueenCardImages));
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
        private ImageSource GetCardImage(int index)
        {
            if (game.QueenTableCards.Count == 0)
                return Strings.greencard;
            if (game.QueenTableCards[index].IsAwaken)
            {
                CardView cardView = new();
                cardView.SetCardSource(game.QueenTableCards[index].Type, game.QueenTableCards[index].Value);
                return cardView.Source;
            }
            else
                return Strings.greencard;
        }
        private void OnGameChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(OpponentName));
            OnPropertyChanged(nameof(StatusMessage));
            OnPropertyChanged(nameof(OpenedCardImage));
            OnPropertyChanged(nameof(OpenedCardVisible));
            OnPropertyChanged(nameof(QueenCardImages));
            OnPropertyChanged(nameof(game.IsHostTurn));
            if (game.DeckCards.Count == 61 && game.IsHostUser)
            {
                for (int i= 0; i < 5; i++)
                    TakePackageCard();
                game.UpdateFbInGame(OnCompleteUpdate);
                OnPropertyChanged(nameof(GameStarted));
            }
            if (game.IllegalMove && game.StatusMessage == Strings.PlayMessage)
            {
                Toast.Make(Strings.IllegalMove, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                game.IllegalMove = false;
            }
            if (game.Equation != string.Empty && game.StatusMessage == Strings.PlayMessage)
                game.Equation = string.Empty;
            if (game.Equation != string.Empty && game.StatusMessage == Strings.WaitMessage)
            {
                Toast.Make(game.Equation, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                game.Equation = string.Empty;
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

