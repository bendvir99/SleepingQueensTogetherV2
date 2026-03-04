using SleepingQueensTogether.Models;

namespace SleepingQueensTogether.ModelsLogic
{
    public class CardsSet : CardsSetModel
    {
        // במחלקה הזאת מתנהל כל הפעולות של החבילת קלפים וכל מה שאפשר לעשות עם החבילה
        #region Constructors
        public CardsSet(bool full) : base()
        {
            // הפעולה מקבלת בוליאן האם למלא את החבילה או לא. הפעולה לא מחזירה שום ערך
            if (full)
                FillPackage();
        }
        public CardsSet() : base() { }
        #endregion

        #region Public Methods
        public override void FillPackage()
        {
            // הפעולה לא מקבלת שום פרמטרים ולא מחזירה שום ערך
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 10; j++)
                    CardsDeck.Add(new Card(Strings.number, j + 1));
            for (int i = 0; i < 7; i++)
                CardsDeck.Add(new Card(Strings.king, -(i + 1)));
            for (int i = 0; i < 4; i++)
                CardsDeck.Add(new Card(Strings.knight, -(i + 1)));
            for (int i = 0; i < 3; i++)
                CardsDeck.Add(new Card(Strings.dragon, -1));
            for (int i = 0; i < 5; i++)
                CardsDeck.Add(new Card(Strings.joker, -1));
            for (int i = 0; i < 4; i++)
                CardsDeck.Add(new Card(Strings.sleepingpotion, -1));
            for (int i = 0; i < 3; i++)
                CardsDeck.Add(new Card(Strings.wand, -1));
        }
        public override Card Add(Card card)
        {
            // הפעולה מקבלת קלף והפעולה מחזירה את אותו קלף אחרי שהפעולה הוסיפה אותו לחבילה
            card.Index = CardsDeck.Count;
            CardsDeck.Add(card);
            return card;
        }
        public override Card TakeCard()
        {
            // הפעולה לא מקבלת שום פרמטרים ומחזירה קלף רנדומלי שנלקח מהחבילה
            Card card = new();
            if (CardsDeck.Count > 0)
            {
                int index = rnd.Next(0, CardsDeck.Count);
                card = CardsDeck[index];
                CardsDeck.RemoveAt(index);
            }
            return card;
        }
        public override void SelectCard(Card card)
        {
            // הפעולה מקבלת קלף ולא מחזירה שום ערך
            if (card.IsSelected)
            {
                SelectedCards.Remove(card);
                card?.ToggleSelected();
            }
            else
            {
                SelectedCards.Add(card);
                card?.ToggleSelected();
            }
        }
        public override List<Card> ThrowCard()
        {
            // הפעולה לא מקבלת שום פרמטרים ומחזירה רשימה של כל הקלפים שזרקנו לשולחן
            List<Card> card = [];
            for (int i = 0; i < SelectedCards.Count; i++)
            {
                card.Add(Card.Copy(SelectedCards[i]));
                if (!SelectedCards[i].IsEmpty)
                {
                    CardsDeck.Remove(SelectedCards[i]);
                    for (int j = SelectedCards[i].Index; j < CardsDeck.Count; j++)
                    {
                        CardsDeck[j].Index = j;
                        CardsDeck[j].Margin = new Thickness(0, 0, 0, 0);
                    }
                }
            }
            SelectedCards.Clear();
            return card;
        }
        #endregion
    }
}
