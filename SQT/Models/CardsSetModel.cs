using Plugin.CloudFirestore.Attributes;
using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    public abstract class CardsSetModel
    {
        // מחלקת המודל של קבוצת קלפים (חבילה או הקלפים ביד) שיש בה את כל הנתונים של החבילה כולל החבילה עצמה
        #region Fields
        protected Random rnd = new();
        #endregion

        #region Properties
        public List<Card> CardsDeck { get; protected set; } = [];
        public List<Card> SelectedCards { get; protected set; } = [];
        public CardsSetModel() { CardsDeck = []; }
        public abstract void FillPackage();
        public abstract Card Add(Card card);
        public abstract Card TakeCard();
        public abstract void SelectCard(Card card);
        public abstract List<Card> ThrowCard();
        [Ignored]
        public int Count => CardsDeck.Count;
        #endregion
    }
}
