using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.Platforms.Android
{
    public class MyTimer(long millisInFuture, long countDownInterval) : CountDownTimer(millisInFuture, countDownInterval)
    {
        // מחלקת הטיימר עם פעולות של סיום וכשהטיימר פועל
        #region Public Methods
        public override void OnFinish()
        {
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(Keys.FinishedSignal));
        }
        public override void OnTick(long millisUntilFinished)
        {
            WeakReferenceMessenger.Default.Send(new AppMessage<long>(millisUntilFinished));
        }
        #endregion
    }
}
