using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace SleepingQueensTogether.Models
{
    public class General
    {
        // מחלקה כללית עם פעולות כלליות
        #region Public Methods
        public static void ToastMake(string message)
        {
            // הפעולה מקבלת הודעה כדי לעשות לה טוסט ולא מחזירה שום ערכים
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Toast.Make(message, ToastDuration.Long, 14).Show();
            });

        }
        #endregion
    }
}
