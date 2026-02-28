using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Firebase.Auth.Requests;
using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using System.Reflection;
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
        public string Total => $"{Strings.TotalQueens}" + game.QueensCount + $"\n{Strings.TotalPoints}" + game.QueenPoints;
        public bool GameStarted => game.DeckCards.Count < 66;
        public bool OpenedCardVisible => game.OpenedCard.Type != Strings.empty;
        public ImageSource?[] QueenTableCardImages => [.. Enumerable.Range(0, 12).Select(i => GetCardImage(i, 1))];
        public ImageSource?[] QueenCardImages => [.. Enumerable.Range(0, 5).Select(i => GetCardImage(i, 2))];
        public ImageSource?[] OpponentQueenCardImages => [.. Enumerable.Range(0, 5).Select(i => GetCardImage(i, 3))];
        public bool CanStartGame => CanStart();
        public ICommand StartGameCommand { get; }
        public ICommand SelectCardCommand { get; }
        public ICommand ThrowSelectedCardCommand { get; }
        public ICommand EndTurnCommand { get; }
        public ICommand TakeQueenCardCommand { get; }
        public ICommand ClickQueenCardCommand { get; }

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
            game.TimesUp += OnTimesUp;
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

        private void ClickQueenCard(string index)
        {
            int i = int.Parse(index);
            if (game.CanClickQueen && game.OpponentQueenCards.Count > i)
            {
                if (game.OpenedCard.Type == Strings.knight)
                {
                    game.QueenCards.Add(game.OpponentQueenCards[i]);
                    game.QueenPoints += game.OpponentQueenCards[i].QueenValue;
                    game.QueensCount++;
                }
                else
                {
                    for (int j = 0; j < game.QueenTableCards.Count; j++)
                    {
                        if (game.QueenTableCards[j].Value == game.OpponentQueenCards[i].Value)
                        {
                            game.QueenTableCards[j].IsAwaken = false;
                            break;
                        }
                    }
                }
                if (game.QueensCount >= 5)
                {
                    game.UpdateMessage = Strings.OpponentWon + game.QueensCount + Strings.Queens;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Toast.Make(Strings.YouWon + game.QueensCount + Strings.Queens, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    });
                    EndGame();
                }
                else if (game.QueenPoints >= 50)
                {
                    game.UpdateMessage = Strings.OpponentWon + game.QueenPoints + Strings.Points;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Toast.Make(Strings.YouWon + game.QueenPoints + Strings.Points, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    });
                    EndGame();
                }

                game.OpponentQueenCards.Remove(game.OpponentQueenCards[i]);
                game.RemoveQueen = true;
                game.RemoveQueenIndex = i;
                game.CanClickQueen = false;
                game.UpdateFbInGame(OnCompleteUpdate);
                game.RemoveQueen = false;
                game.RemoveQueenIndex = -1;
                OnPropertyChanged(nameof(OpponentQueenCardImages));
                OnPropertyChanged(nameof(QueenCardImages));
                OnPropertyChanged(nameof(Total));
            }
        }

        private async static void EndGame()
        {
            await Task.Delay(3000);
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PopAsync();
            });
        }

        private void TakeQueenCard(string index)
        {
            int i = int.Parse(index);
            if (game.CanPickQueen && !game.QueenTableCards[i].IsAwaken)
            {
                game.TakeQueenCard(i);
                if (game.QueensCount >= 5)
                {
                    game.UpdateMessage = Strings.OpponentWon + game.QueensCount + Strings.Queens;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Toast.Make(Strings.YouWon + game.QueensCount + Strings.Queens, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    });
                    EndGame();
                }
                else if (game.QueenPoints >= 50)
                {
                    game.UpdateMessage = Strings.OpponentWon + game.QueenPoints + Strings.Points;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Toast.Make(Strings.YouWon + game.QueenPoints + Strings.Points, CommunityToolkit.Maui.Core.ToastDuration.Long, 14).Show();
                    });
                    EndGame();
                }
                game.UpdateFbInGame(OnCompleteUpdate);
                OnPropertyChanged(nameof(QueenTableCardImages));
                OnPropertyChanged(nameof(QueenCardImages));
                OnPropertyChanged(nameof(Total));

            }
        }


        private void EndTurn()
        {
            if (!game.CanPickQueen && game.StatusMessage == Strings.PlayMessage && game.myCards.CardsDeck.Count < 5 && !game.CanClickQueen)
            {
                while (game.myCards.CardsDeck.Count < 5)
                {
                    TakePackageCard();
                }
                if (game.QueenPoints >= 50 || game.QueensCount >= 5)
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Shell.Current.Navigation.PopAsync();
                    });
                }
                else
                {
                    game.KnightPlaced = false;
                    game.PotionPlaced = false;
                    game.ChangeTurn();
                    game.UpdateFbInGame(OnCompleteUpdate);
                    OnPropertyChanged(nameof(game.IsHostTurn));
                }
            }
        }

        private void ThrowSelectedCard()
        {
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
            if (args.SelectedCard != null && args.SelectedCardView != null)
            {
                game.SelectCard(args.SelectedCard);
                args.SelectedCardView.Margin = args.SelectedCard.Margin;
            }
        }

        private void OnTimeLeftChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Timeleft));
        }

        private void OnTimesUp(object? sender, EventArgs e)
        {
            for (int i = 0; i < stkMyCards.Children.Count; i++)
                if (stkMyCards.Children[i].Margin != new Thickness(0))
                    if (stkMyCards.Children[i] is ImageButton imagebutton && imagebutton.Command != null)
                        imagebutton.Command.Execute(imagebutton.CommandParameter);
            while (game.myCards.CardsDeck.Count < 5)
            {
                TakePackageCard();
            }
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
        private ImageSource? GetCardImage(int index, int type)
        {
            if (type == 1)
            {
                if (game.QueenTableCards.Count == 0)
                    return Strings.greencard;
                if (index >= game.QueenTableCards.Count)
                    return null;
                if (game.QueenTableCards[index].IsAwaken)
                    return null;
                else
                    return Strings.greencard;
            }
            else if (type == 2)
            {
                if (index >= game.QueenCards.Count)
                    return null;
                if (game.QueenCards.Count == 0)
                    return null;
                if (game.QueenCards[index] != null)
                {
                    CardView cardView = new();
                    cardView.SetCardSource(game.QueenCards[index].Type, game.QueenCards[index].Value);
                    return cardView.Source;
                }
                else
                    return null;
            }
            else
            {
                if (game.OpponentQueenCards.Count == 0)
                    return null;
                if (index >= game.OpponentQueenCards.Count)
                    return null;
                if (game.OpponentQueenCards[index] != null)
                {
                    CardView cardView = new();
                    cardView.SetCardSource(game.OpponentQueenCards[index].Type, game.OpponentQueenCards[index].Value);
                    return cardView.Source;
                }
                else
                    return null;
            }
        }
        private void OnGameChanged(object? sender, EventArgs e)
        {
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

