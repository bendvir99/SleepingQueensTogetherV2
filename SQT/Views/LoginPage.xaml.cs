using SleepingQueensTogether.ViewModels;

namespace SleepingQueensTogether.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = new LoginPageVM();
	}
}