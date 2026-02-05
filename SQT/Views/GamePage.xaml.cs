using SleepingQueensTogether.ModelsLogic;
using SleepingQueensTogether.ViewModels;

namespace SleepingQueensTogether.Views;

public partial class GamePage : ContentPage
{
    private readonly GamePageVM gpVM;
    public GamePage(Game game)
	{
		InitializeComponent();
        gpVM = new GamePageVM(game, stkMyCards);
        BindingContext = gpVM;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        gpVM.AddSnapshotListener();
    }

    protected override void OnDisappearing()
    {
        gpVM.RemoveSnapshotListener();
        base.OnDisappearing();
    }
}