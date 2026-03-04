using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SleepingQueensTogether.Models
{
    // מחלקה להודעות
    public class AppMessage<T>(T msg) : ValueChangedMessage<T>(msg) { }
}
