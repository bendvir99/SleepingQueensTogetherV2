using SleepingQueensTogether.Models;
using Xamarin.Google.Crypto.Tink.Prf;

namespace SleepingQueensTogether.ModelsLogic
{
    public class Card : CardModel
    {
      
        public Card()
        {
            Type = Strings.empty;
        }
        public Card(string type, int value)
        {
            Type = type;
            Value = value;            
        }
        public override void ToggleSelected()
        {
            IsSelected = !IsSelected;
            Thickness t = Margin;
            t.Bottom = IsSelected ? Offset : 0;
            Margin = t;
        }
        public static Card Copy(Card card)
        {
            Card newCard = new();
            if (!card.IsEmpty)
            {
                newCard = new Card(card.Type, card.Value)
                {
                    Index = card.Index
                };
            }
            return newCard;
        }
    }
}
