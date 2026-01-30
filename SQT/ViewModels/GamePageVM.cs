using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.Maui.Biometric;
using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public partial class GamePageVM : ObservableObject
    {
        private readonly Game game;
        private readonly StackLayout stkMyCards;
        private readonly ScrollView scrlMyCards;
        public string Timeleft => Strings.TimeLeft + game.TimeLeft;
        public string MyName => game.MyName;
        public string StatusMessage => game.StatusMessage;
        public string OpponentName => game.OpponentName;
        public string Total => $"{Strings.TotalQueens}\n{Strings.TotalPoints}";
        public bool GameStarted => game.Package.Count < 66;
        //public string[] CardImages => [.. game.Cards.Select(c => c.Image)];
        //public string[] QueenCardImages => [.. Enumerable.Range(0, 12).Select(i => GetCardImage(i))];

        public bool CanStartGame => CanStart();
        public ICommand ChangeTurnCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand SelectCardCommand { get; }
        public GamePageVM(Game game, StackLayout stkMyCards, ScrollView scrlMyCards)
        {
            game.GameChanged += OnGameChanged;
            game.TimeLeftChanged += OnTimeLeftChanged;
            ChangeTurnCommand = new Command(ChangeTurn);
            StartGameCommand = new Command(StartGame);
            SelectCardCommand = new Command<SelectCardEventArgs>(SelectCard);
            this.game = game;
            this.stkMyCards = stkMyCards;
            this.scrlMyCards = scrlMyCards;
            if (!game.IsHostUser)
            {
                game.UpdateGuestUser(OnComplete);
            }
        }

        private void SelectCard(SelectCardEventArgs args)
        {
            if (args.SelectedCard != null && args.SelectedCardView != null)
            {
                game.SelectCard(args.SelectedCard);
                args.SelectedCardView.Margin = args.SelectedCard.Margin;
                //OnPropertyChanged(nameof(IsSelectedMatch));
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
            cardView.WidthRequest = 100;
            if (card != null)
            {
                stkMyCards.Children.Add(cardView);
                SelectCardEventArgs scea = new() { SelectedCard = card, SelectedCardView = cardView};
                cardView.CommandParameter = scea;
                cardView.Command = SelectCardCommand;
                scrlMyCards.ScrollToAsync(stkMyCards, ScrollToPosition.End, true);
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
            return !game.IsHostUser && game.Package.Count == 66;
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
            if (game.Package.Count == 61 && game.IsHostUser)
            {
                for (int i= 0; i < 5; i++)
                {
                    Console.WriteLine(game.Package.Count);
                    TakePackageCard();
                }
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

