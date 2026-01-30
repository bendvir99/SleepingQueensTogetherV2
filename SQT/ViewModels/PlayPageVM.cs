using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    class PlayPageVM : ObservableObject
    {
        private readonly Games games = new();
        public bool IsBusy => games.IsBusy;
        public ICommand AddGameCommand => new Command(AddGame);

        private void AddGame()
        {
            if (!IsBusy)
            {
                games.AddGame();
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public Game? SelectedItem
        {
            get => games.CurrentGame;

            set
            {
                if (value != null)
                {
                    games.CurrentGame = value;
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        Shell.Current.Navigation.PushAsync(new GamePage(value), true);
                    });
                }
            }
        }

        public ObservableCollection<Game>? GamesList => games.GamesList;
        public PlayPageVM()
        {
            games.GameAdded += OnGameAdded;
            games.GamesChanged += OnGamesChanged;
        }

        private void OnGamesChanged(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(GamesList));
        }

        private void OnGameAdded(object? sender, Game game)
        {
            OnPropertyChanged(nameof(IsBusy));
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new GamePage(game), true);
            });
        }
        internal void AddSnapshotListener()
        {
            games.AddSnapshotListener();
        }

        internal void RemoveSnapshotListener()
        {
            games.RemoveSnapshotListener();
        }
    }
}
