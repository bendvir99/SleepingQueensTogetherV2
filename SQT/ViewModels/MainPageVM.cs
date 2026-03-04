using SleepingQueensTogether.Models;
using SleepingQueensTogether.Views;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    public class MainPageVM : ObservableObject
    {
        // המחלקה שעוזרת לדף הראשי לפעול
        #region ICommands
        public ICommand PlayCommand { get; }
        public ICommand RulesCommand { get; }
        #endregion

        #region Constructor
        public MainPageVM()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            PlayCommand = new Command(Play);
            RulesCommand = new Command(Rules);
        }
        #endregion

        #region Private Methods
        private void Rules()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new RulesPage(), true);
            });
        }
        private void Play()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new PlayPage(), true);
            });
        }
        #endregion
    }
}
