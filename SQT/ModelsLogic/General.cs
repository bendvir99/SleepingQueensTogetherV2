using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace SleepingQueensTogether.ModelsLogic
{
    public static class General
    {
        public static void ToastMake(string message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Toast.Make(message, ToastDuration.Long).Show();
            });
        }
    }
}
