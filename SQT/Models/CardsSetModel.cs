using Plugin.CloudFirestore.Attributes;
using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    public abstract class CardsSetModel
    {
        public List<Card> CardsDeck { get; set; } = [];
        public CardsSetModel() { CardsDeck = []; }

        public bool SingleSelect { protected get; set; }

        public abstract void FillPackage();
        [Ignored]
        public int Count => CardsDeck.Count;
        
    }
}
