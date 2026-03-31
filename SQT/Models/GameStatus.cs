namespace SleepingQueensTogether.Models
{
    public class GameStatus
    {
        // המחלקה שמתעסקת בתורות ומי משחק
        #region Enums
        public enum Statuses { Wait, Play }
        #endregion

        #region Fields
        private readonly string[] msgs = [Strings.WaitMessage, Strings.PlayMessage];
        #endregion

        #region Properties
        public Statuses CurrentStatus { get; set; } = Statuses.Wait;
        public string StatusMessage => msgs[(int)CurrentStatus];
        #endregion

        #region Public Methods
        public void UpdateStatus()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            CurrentStatus = CurrentStatus == Statuses.Play ? Statuses.Wait : Statuses.Play;
        }
        #endregion
    }
}
