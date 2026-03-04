using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    public class SelectCardEventArgs : EventArgs
    {
        // מחלקה עם נתונים של איוונט של סימון קלף
        #region Properties
        public Card? SelectedCard { get; set; }
        public CardView? SelectedCardView { get; set; }
        #endregion
    }
}
