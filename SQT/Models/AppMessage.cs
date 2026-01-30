using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SleepingQueensTogether.Models
{
    public class AppMessage<T>(T msg) : ValueChangedMessage<T>(msg)
    {

    }
}
