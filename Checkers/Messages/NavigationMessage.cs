using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Messages
{
    public class NavigationMessage : IMessage
    {
        public Type ViewModelType { get; set; }
    }
}
