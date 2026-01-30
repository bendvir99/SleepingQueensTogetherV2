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
        private Card selectedCard;
        private readonly Card emptyCard;

        public CardsSet(bool full) : base()
        {
            emptyCard = new Card();
            selectedCard = emptyCard;
            rnd = new Random();
            if (full)
                FillPackage();

        }
        public CardsSet() : base()
        {
            emptyCard = new Card();
            selectedCard = emptyCard;
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
                CardsDeck.Add(new Card(Strings.king, i + 1));
            }
            for (int i = 0; i < 4; i++)
            {
                CardsDeck.Add(new Card(Strings.knight, i + 1));
            }
            for (int i = 0; i < 3; i++)
            {
                CardsDeck.Add(new Card(Strings.dragon, 1));
            }
            for (int i = 0; i < 5; i++)
            {
                CardsDeck.Add(new Card(Strings.joker, 1));
            }
            for (int i = 0; i < 4; i++)
            {
                CardsDeck.Add(new Card(Strings.sleepingpotion, 1));
            }
            for (int i = 0; i < 3; i++)
            {
                CardsDeck.Add(new Card(Strings.wand, 1));
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
            if (SingleSelect)
                if (card.IsSelected)
                {
                    selectedCard = emptyCard;
                    card?.ToggleSelected();
                }
                else
                {
                    selectedCard?.ToggleSelected();
                    card.ToggleSelected();
                    selectedCard = card;
                }
            else
                card?.ToggleSelected();
        }

        public Card ThrowCard(Card comparedCard)
        {
            Card card = new();
            if (IsMatch(comparedCard))
            {
                card = Card.Copy(selectedCard);
                if (!selectedCard.IsEmpty)
                {
                    CardsDeck.Remove(selectedCard);
                    for (int i = selectedCard.Index; i < CardsDeck.Count; i++)
                    {
                        CardsDeck[i].Index = i;
                        CardsDeck[i].Margin = new Thickness(CardsDeck[i].Margin.Left - 30, 0, 0, 0);
                    }
                    selectedCard = emptyCard;
                }
            }
            return card;
        }
        public bool IsMatch(Card card)
        {
            bool match = false;
            if (!selectedCard.IsEmpty)
                match = card.Value == selectedCard.Value || card.Type == selectedCard.Type;
            return match;
        }
    }
}
