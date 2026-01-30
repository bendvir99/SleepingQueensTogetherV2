using SleepingQueensTogether.ModelsLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepingQueensTogether.Models
{
    public class SelectCardEventArgs : EventArgs
    {
        public Card? SelectedCard { get; set; }
        public CardView? SelectedCardView { get; set; }
    }
}
