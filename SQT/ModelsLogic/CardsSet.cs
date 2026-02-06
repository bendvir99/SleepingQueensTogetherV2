using SleepingQueensTogether.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepingQueensTogether.ModelsLogic
{
    public class CardsSet : CardsSetModel
    {
        private readonly Random rnd;

        public CardsSet(bool full) : base()
        {
            rnd = new Random();
            if (full)
                FillPackage();

        }
        public CardsSet() : base()
        {
            rnd = new Random();
        }

        public override void FillPackage()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    CardsDeck.Add(new Card(Strings.number, j + 1));
                }
            }
            for (int i = 0; i < 7; i++)
            {
                CardsDeck.Add(new Card(Strings.king, -(i + 1)));
            }
            for (int i = 0; i < 4; i++)
            {
                CardsDeck.Add(new Card(Strings.knight, -(i + 1)));
            }
            for (int i = 0; i < 3; i++)
            {
                CardsDeck.Add(new Card(Strings.dragon, -1));
            }
            for (int i = 0; i < 5; i++)
            {
                CardsDeck.Add(new Card(Strings.joker, -1));
            }
            for (int i = 0; i < 4; i++)
            {
                CardsDeck.Add(new Card(Strings.sleepingpotion, -1));
            }
            for (int i = 0; i < 3; i++)
            {
                CardsDeck.Add(new Card(Strings.wand, -1));
            }
        }

        public void Reset(bool full)
        {
            //cards.Clear();
            //if (full)
            //    FillPakage();
        }

        public Card Add(Card card)
        {
            card.Index = CardsDeck.Count;
            CardsDeck.Add(card);
            return card;
        }

        public Card TakeCard()
        {
            Card card = new();
            if (CardsDeck.Count > 0)
            {
                int index = rnd.Next(0, CardsDeck.Count);
                card = CardsDeck[index];
                CardsDeck.RemoveAt(index);
            }
            return card;
        }

        public void SelectCard(Card card)
        {
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

        public List<Card> ThrowCard()
        {
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
    }
}
