using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepingQueensTogether.Models
{
    public class ToastMessageEventArgs(string message) : EventArgs
    {
        public string message { get; } = message;
    }
}
