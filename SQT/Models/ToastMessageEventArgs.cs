namespace SleepingQueensTogether.Models
{
    public class ToastMessageEventArgs(string message) : EventArgs
    {
        // מחלקה עם נתונים על האיוונט של ההודעות
        #region Properties
        public string message { get; } = message;
        #endregion
    }
}
