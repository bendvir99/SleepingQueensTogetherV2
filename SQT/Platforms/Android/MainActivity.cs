using Android.App;
using Android.Content;
using Android.Content.PM;
using CommunityToolkit.Mvvm.Messaging;
using SleepingQueensTogether.Models;
using SleepingQueensTogether.Platforms.Android;

namespace SleepingQueensTogether
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        MyTimer? mTimer;
        override protected void OnCreate(Android.OS.Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RegisterTimeMessage();
            StartDeleteFbDocsService();
        }

        private void StartDeleteFbDocsService()
        {
            Intent intent = new(this, typeof(DeleteFbDocsService));
            StartService(intent);
        }

        private void RegisterTimeMessage()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<TimerSettings>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
            WeakReferenceMessenger.Default.Register<AppMessage<bool>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }

        private void OnMessageReceived(bool value)
        {
            if (value)
            {
                mTimer?.Cancel();
                mTimer = null;
            }
        }

        private void OnMessageReceived(TimerSettings value)
        {
            mTimer = new MyTimer(value.TotalTimeInMilliseconds, value.IntervalInMilliseconds);
            mTimer.Start();
        }
    }
}
