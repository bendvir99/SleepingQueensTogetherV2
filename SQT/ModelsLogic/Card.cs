using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    public class Card : CardModel
    {
        // במחלקה הזאת מתנהל כל הפעולות של הקלף וכל מה שאפשר לעשות עם קלף
        #region Constructors
        public Card()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            Type = Strings.empty;
        }
        public Card(string type, int value)
        {
            // הפעולה מקבלת את סוג הקלף ואת ערכו. הפעולה לא מחזירה שום ערך
            Type = type;
            Value = value;
            if (type == Strings.queen)
                QueenValue = new[] { 5, 15, 15, 20, 10, 10, 15, 10, 5, 5, 5, 10 }[value];
        }
        #endregion

        #region Public Methods
        public override void ToggleSelected()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            IsSelected = !IsSelected;
            Thickness t = Margin;
            t.Bottom = IsSelected ? Offset : 0;
            Margin = t;
        }
        public static Card Copy(Card card)
        {
            // הפעולה מקבלת קלף ומחזירה שכפול של אותו קלף
            Card newCard = new();
            if (!card.IsEmpty)
                newCard = new Card(card.Type, card.Value) { Index = card.Index };
            return newCard;
        }
        #endregion
    }
}
