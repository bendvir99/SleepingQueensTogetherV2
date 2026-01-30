using SleepingQueensTogether.ViewModels;

namespace SleepingQueensTogether.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
        InitializeComponent();
		BindingContext = new RegisterPageVM();
	}
}