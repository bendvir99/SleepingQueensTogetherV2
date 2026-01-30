using SleepingQueensTogether.ViewModels;

namespace SleepingQueensTogether.Views;

public partial class PlayPage : ContentPage
{
    private readonly PlayPageVM ppVM = new();
    public PlayPage()
	{
		InitializeComponent();
        BindingContext = ppVM;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        ppVM.AddSnapshotListener();
    }

    protected override void OnDisappearing()
    {
        ppVM.RemoveSnapshotListener();
        base.OnDisappearing();
    }
}