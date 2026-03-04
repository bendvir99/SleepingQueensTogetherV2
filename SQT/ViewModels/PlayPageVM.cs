using SleepingQueensTogether.Models;
using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public class PlayPageVM : ObservableObject
    {
        // המחלקה שעוזרת לדף המשחק לפעול
        #region Fields
        private readonly Games games = new();
        #endregion

        #region ICommands
        public ICommand AddGameCommand => new Command(AddGame);
        #endregion

        #region Properties
        public bool IsBusy => games.IsBusy;
        public ObservableCollection<Game>? GamesList => games.GamesList;
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
        #endregion

        #region Constructor
        public PlayPageVM()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            games.GameAdded += OnGameAdded;
            games.GamesChanged += OnGamesChanged;
        }
        #endregion

        #region Public Methods
        public void AddSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            games.AddSnapshotListener();
        }
        public void RemoveSnapshotListener()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            games.RemoveSnapshotListener();
        }
        #endregion

        #region Private Methods
        private void AddGame()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            if (!IsBusy)
            {
                games.AddGame();
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        private void OnGamesChanged(object? sender, EventArgs e)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת הנתונים של האיוונט. הפעולה לא מחזירה שום ערך
            OnPropertyChanged(nameof(GamesList));
        }
        private void OnGameAdded(object? sender, Game game)
        {
            // הפעולה מקבלת את מי ששלח את האיוונט ואת המשחק. הפעולה לא מחזירה שום ערך
            OnPropertyChanged(nameof(IsBusy));
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new GamePage(game), true);
            });
        }
        #endregion
    }
}
