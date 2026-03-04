namespace SleepingQueensTogether.Models
{
    public class TimerSettings(long totalTimeInMilliseconds, long intervalInMilliseconds)
    {
        // במחלקה זו יש מידע על הטיימר הראשי
        #region Properties
        public long TotalTimeInMilliseconds { get; } = totalTimeInMilliseconds;
        public long IntervalInMilliseconds { get; } = intervalInMilliseconds;
        #endregion
    }
}
