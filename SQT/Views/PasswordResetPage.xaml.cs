using SleepingQueensTogether.ViewModels;

namespace SleepingQueensTogether.Views;

public partial class PasswordResetPage : ContentPage
{
	public PasswordResetPage()
	{
		InitializeComponent();
		BindingContext = new PasswordResetVM();
    }
}