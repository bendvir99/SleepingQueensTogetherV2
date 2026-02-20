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
            if (type == Strings.queen)
            {
                switch (value)
                {
                    case 0:
                        QueenValue = 5;
                        break;
                    case 1:
                        QueenValue = 15;
                        break;
                    case 2:
                        QueenValue = 15;
                        break;
                    case 3:
                        QueenValue = 20;
                        break;
                    case 4:
                        QueenValue = 10;
                        break;
                    case 5:
                        QueenValue = 10;
                        break;
                    case 6:
                        QueenValue = 15;
                        break;
                    case 7:
                        QueenValue = 10;
                        break;
                    case 8:
                        QueenValue = 15;
                        break;
                    case 9:
                        QueenValue = 5;
                        break;
                    case 10:
                        QueenValue = 5;
                        break;
                    case 11:
                        QueenValue = 10;
                        break;
                }
            }
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
                newCard = new Card(card.Type, card.Value) { Index = card.Index };
            return newCard;
        }
    }
}
