using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Messages
{
    public class SettingInfoMessage : IMessage
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public int FirstMove { get; set; }
    }
}
