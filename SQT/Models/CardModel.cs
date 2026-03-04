using Plugin.CloudFirestore.Attributes;

namespace SleepingQueensTogether.Models
{
    public abstract class CardModel
    {
        // מחלקת המודל של הקלף עם כל תכונות הקלף
        #region Fields
        protected const int Offset = 50;
        #endregion

        #region Properties
        public string Type { get; protected set; } = Strings.number;
        public int Value { get; protected set; } = 0;
        public bool IsAwaken { get; set; } = false;
        public int QueenValue { get; protected set; } = 0;
        [Ignored]
        public int Index { get; set; }
        [Ignored]
        public Thickness Margin { get; set; }
        [Ignored]
        public bool IsSelected { get; protected set; } = false;
        [Ignored]
        public bool IsEmpty => Type == Strings.empty;
        #endregion

        #region Public Methods
        public abstract void ToggleSelected();
        #endregion
    }
}
