namespace SleepingQueensTogether.Models
{
    public class GameStatus
    {
        private readonly string[] msgs = [Strings.WaitMessage, Strings.PlayMessage];
        public enum Statuses { Wait, Play }
        public Statuses CurrentStatus { get; set; } = Statuses.Wait;
        public string StatusMessage => msgs[(int)CurrentStatus];
        public void UpdateStatus()
        {
            CurrentStatus = CurrentStatus == Statuses.Play ? Statuses.Wait : Statuses.Play;
        }
    }
}
