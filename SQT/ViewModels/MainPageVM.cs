using SleepingQueensTogether.Models;
using SleepingQueensTogether.Views;
using System.Windows.Input;

namespace SleepingQueensTogether.ViewModels
{
    class MainPageVM : ObservableObject
    {
        public ICommand PlayCommand { get; }
        public ICommand RulesCommand { get; }
        public MainPageVM()
        {
            PlayCommand = new Command(Play);
            RulesCommand = new Command(Rules);
        }

        private void Rules()
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new RulesPage(), true);
            });
        }

        private void Play()
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                Shell.Current.Navigation.PushAsync(new PlayPage(), true);
            });
        }
    }
}
