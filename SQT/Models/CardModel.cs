using Kotlin.Collections;
using Plugin.CloudFirestore.Attributes;
using SleepingQueensTogether.ModelsLogic;

namespace SleepingQueensTogether.Models
{
    public abstract class CardModel
    {
        protected const int Offset = 50;
        public string Type { get; set; } = Strings.number;
        public int Value { get; set; } = 0;
        public bool IsUsed { get; set; } = false;
        public bool IsAwaken { get; set; } = false;
        public int QueenValue { get; set; } = 0;
        [Ignored]
        public int Index { get; set; }
        [Ignored]
        public Thickness Margin { get; set; }
        [Ignored]
        public bool IsSelected { get; protected set; } = false;
        [Ignored]
        public bool IsEmpty => Type == Strings.empty;
        public abstract void ToggleSelected();
    }
}
