using SleepingQueensTogether.ViewModels;

namespace SleepingQueensTogether
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageVM mpVM = new();
        public MainPage()
        {
            InitializeComponent();
            BindingContext = mpVM;
        }
    }
}
