namespace SleepingQueensTogether.Models
{
    public class TimerSettings(long totalTimeInMilliseconds, long intervalInMilliseconds)
    {
        public long TotalTimeInMilliseconds { get; set; } = totalTimeInMilliseconds;
        public long IntervalInMilliseconds { get; set; } = intervalInMilliseconds;
    }
}
