using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace SleepingQueensTogether.Models;
public partial class ObservableObject : INotifyPropertyChanged
{
    // class that handles property changed between vms and views
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
