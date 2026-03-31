using SleepingQueensTogether.ViewModels;

namespace SleepingQueensTogether.Views;

public partial class RulesPage : ContentPage
{
	public RulesPage()
	{
		InitializeComponent();
		BindingContext = new RulesPageVM();
    }
}